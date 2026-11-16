using Droosy_DAL.Data;
using Droosy_DAL.Entities;

namespace Droosy_DAL.Seeding;

public static class DemoFeatureSeed
{
 public static void Add(ApplicationDbContext db, DateTime anchor)
 {
  var assignments=new[] {
   new Assignment{Id="ALG-W01",CourseId="ALG",Title="معادلات من خطوة واحدة",Instructions="حل x + 5 = 12 و3x = 18، مع كتابة خطوات الحل.",DueAtUtc=anchor.AddDays(-2)},
   new Assignment{Id="ALG-W02",CourseId="ALG",Title="تدريب المعادلات",Instructions="حل 2x + 3 = 11 و3(x + 1) = 12، مع توضيح الخطوات.",DueAtUtc=anchor.AddDays(2)},
   new Assignment{Id="GEO-W01",CourseId="GEO",Title="الزوايا والمثلثات",Instructions="أوجد الزاوية المكملة لـ70°، ثم الزاوية الثالثة لمثلث زاويتاه 50° و60°.",DueAtUtc=anchor.AddDays(-1)},
   new Assignment{Id="GEO-W02",CourseId="GEO",Title="المحيط والمساحة",Instructions="احسب محيط مستطيل طوله 6 وعرضه 4، ومساحة مربع طول ضلعه 5. اكتب الوحدة المناسبة.",DueAtUtc=anchor.AddDays(3)}
  };
  db.Assignments.AddRange(assignments);
  var submissions=new[] {
   Make("ALG-W01","S03",SubmissionStatus.Graded,8,true,"x = 7، وx = 6. بالتعويض نحصل على 12 و18.","الحل صحيح مع خطوة ناقصة"),
   Make("ALG-W01","S04",SubmissionStatus.Returned,null,false,"x = 7، وx = 6.","أكمل خطوات الحل"),
   Make("ALG-W01","S05",SubmissionStatus.Submitted,null,false,"نطرح 5 من الطرفين: x = 7. نقسم على 3: x = 6.",""),
   Make("ALG-W02","S03",SubmissionStatus.Draft,null,false,"2x = 8، إذن x = 4. سأكمل السؤال الثاني.",""),
   Make("ALG-W02","S05",SubmissionStatus.Submitted,null,false,"2x = 8، x = 4. وفي السؤال الثاني x + 1 = 4، إذن x = 3.",""),
   Make("GEO-W01","S02",SubmissionStatus.Graded,9,true,"180 - 70 = 110. الزاوية الثالثة = 180 - 50 - 60 = 70.","راجع كتابة وحدة الزاوية"),
   Make("GEO-W01","S06",SubmissionStatus.Graded,7,false,"المكملة = 180 - 70 = 110°. والثالثة = 180 - (50 + 60)، وحسبتها 80°.","راجع جمع زاويتي المثلث"),
   Make("GEO-W01","S07",SubmissionStatus.Submitted,null,false,"المكملة = 110°، والزاوية الثالثة = 70°.",""),
   Make("GEO-W01","S08",SubmissionStatus.Graded,0,true,"المكملة = 20°، والزاوية الثالثة = 30°.","الإجابتان غير صحيحتين؛ راجع قاعدة 180°"),
   Make("GEO-W02","S02",SubmissionStatus.Draft,null,false,"محيط المستطيل = 2 × (6 + 4) = 20 وحدة.","")
  };
  foreach(var sub in submissions) {
   var assignment=assignments.Single(x=>x.Id==sub.AssignmentId);
   sub.SubmittedAtUtc=sub.Status==SubmissionStatus.Draft?null:new[]{assignment.DueAtUtc.AddHours(-5),anchor.AddHours(-3)}.Min();
   if(sub.Status==SubmissionStatus.Graded)sub.GradedAtUtc=anchor.AddHours(-12);
   if(sub.PublishedAtUtc.HasValue)sub.PublishedAtUtc=anchor.AddHours(-10);
  }
  db.AssignmentSubmissions.AddRange(submissions);
  var exams=BuildExams(anchor);
  db.Exams.AddRange(exams);
  var scores=new Dictionary<string,int[]> {
   ["S01"]=[4,6,-1],["S03"]=[8,8,10],["S04"]=[2,4,6],["S05"]=[6,6,8],
   ["S02"]=[0,6,8],["S06"]=[6,8,8],["S07"]=[4,4,6],["S08"]=[8,10,10]};
  foreach(var member in DemoSeedData.Subscriptions)for(int n=1;n<=3;n++) {
   var score=scores[member.Student][n-1];if(score<0)continue;
   var exam=exams.Single(x=>x.Id==member.Course+"-X0"+n);
   var attempt=MakeAttempt(exam,member.Student,score,exam.OpensAtUtc.AddMinutes(10),true);
   attempt.PublishedAtUtc=n==3?anchor.AddDays(-4):exam.ClosesAtUtc.AddHours(1);
   db.ExamAttempts.Add(attempt);
  }
  var live=exams.Single(x=>x.Id=="ALG-X04");
  db.ExamAttempts.Add(MakeAttempt(live,"S03",8,anchor.AddHours(-3),false));
  var ongoing=MakeAttempt(live,"S05",4,anchor.AddMinutes(-1),false);
  ongoing.Answers=ongoing.Answers.Take(2).ToList();ongoing.Score=null;ongoing.SubmittedAtUtc=null;
  db.ExamAttempts.Add(ongoing);
  db.AuditLogs.AddRange(new AuditLog{Id="AUDIT-W01",ActorId="T01",EntityType=nameof(AssignmentSubmission),EntityId="GEO-W01-S02",Action="PublishAssignment",CreatedAtUtc=anchor.AddHours(-10),BeforeJson="{}",AfterJson="{\"Score\":9}"},
   new AuditLog{Id="AUDIT-X03",ActorId="T01",EntityType=nameof(ExamAttempt),EntityId="GEO-X03-S02",Action="PublishExam",CreatedAtUtc=anchor.AddDays(-4),BeforeJson="{}",AfterJson="{\"Score\":8}"});
  AddMessages(db,anchor);
 }
 private static AssignmentSubmission Make(string assignment,string student,SubmissionStatus status,int? score,bool published,string text,string feedback)=>new() {
  Id=assignment+"-"+student,AssignmentId=assignment,StudentId=student,Status=status,Score=score,
  PublishedAtUtc=published?DateTime.UtcNow:null,PublishedScore=published?score:null,PublishedFeedback=published?feedback:"",
  AnswerText=text,Feedback=status==SubmissionStatus.Returned?"":feedback,ReturnReason=status==SubmissionStatus.Returned?feedback:""
 };
 public static List<Exam> BuildExams(DateTime anchor) {
  var result=new List<Exam>();
  string[][] algebra=[ ["x + 3 = 7","2x = 10","2x + 1 = 7","2(x + 1) = 6","مع أحمد 4 أقلام، اشترى عددًا فأصبح لديه 10. كم اشترى؟"],
   ["x + 4 = 9","3x = 18","3x + 2 = 14","3(x + 1) = 12","معه 5 أقلام، فأصبح لديه 12. كم اشترى؟"],
   ["x + 5 = 11","4x = 28","4x + 3 = 23","4(x + 1) = 20","معه 6 أقلام، فأصبح لديه 14. كم اشترى؟"],
   ["x + 5 = 12","3x = 18","2x + 3 = 11","3(x + 1) = 12","معه 4 أقلام، فأصبح لديه 13. كم اشترى؟"] ];
  int[][] algAnswers=[[4,5,3,2,6],[5,6,4,3,7],[6,7,5,4,8],[7,6,4,3,9]];
  string[][] geo=[ ["مكملة زاوية قياسها 70°؟","متممة زاوية قياسها 30°؟","ثالثة مثلث زاويتاه 50° و60°؟","محيط مستطيل 6 × 4؟","مساحة مربع ضلعه 5؟"],
   ["مكملة زاوية قياسها 80°؟","متممة زاوية قياسها 40°؟","ثالثة مثلث زاويتاه 60° و70°؟","محيط مستطيل 7 × 3؟","مساحة مربع ضلعه 6؟"],
   ["مكملة زاوية قياسها 60°؟","متممة زاوية قياسها 20°؟","ثالثة مثلث زاويتاه 40° و80°؟","محيط مستطيل 8 × 5؟","مساحة مربع ضلعه 4؟"],
   ["مكملة زاوية قياسها 50°؟","متممة زاوية قياسها 35°؟","ثالثة مثلث زاويتاه 45° و65°؟","محيط مستطيل 9 × 4؟","مساحة مربع ضلعه 7؟"] ];
  int[][] geoAnswers=[[110,60,70,20,25],[100,50,50,20,36],[120,70,60,26,16],[130,55,70,26,49]];
  foreach(var course in new[]{"ALG","GEO"})for(int n=0;n<4;n++) {
   var date=anchor.AddDays(n==0?-21:n==1?-14:-7);
   var exam=new Exam{Id=course+"-X0"+(n+1),CourseId=course,Title=(course=="ALG"?"اختبار المعادلات ":"اختبار الهندسة ")+(n+1),OpensAtUtc=n<3?date:course=="ALG"?anchor.AddDays(-1):anchor.AddDays(1),ClosesAtUtc=n<3?date.AddHours(2):course=="ALG"?anchor.AddDays(7):anchor.AddDays(2)};
   for(int q=0;q<5;q++) {
    var answer=course=="ALG"?algAnswers[n][q]:geoAnswers[n][q];var suffix=course=="GEO"?(q<3?"°":q==3?" وحدة":" وحدة²"):"";
    string explanation=course=="ALG"?q switch {0=>"نطرح العدد الثابت من الطرفين.",1=>"نقسم الطرفين على معامل x.",2=>"نطرح العدد الثابت، ثم نقسم على معامل x.",3=>"نقسم على العدد خارج القوس، ثم نطرح العدد داخل القوس.",_=>"نطرح عدد الأقلام الأصلي من العدد النهائي."}:q switch {0=>"الزاويتان المتكاملتان مجموعهما 180°؛ نطرح الزاوية المعروفة.",1=>"الزاويتان المتتامتان مجموعهما 90°؛ نطرح الزاوية المعروفة.",2=>"مجموع زوايا المثلث 180°؛ نطرح مجموع الزاويتين.",3=>"المحيط = 2 × (الطول + العرض).",_=>"المساحة = طول الضلع × نفسه."};
    var question=new ExamQuestion{Id=exam.Id+"-Q"+(q+1),Text=course=="ALG"?algebra[n][q]:geo[n][q],Explanation=explanation+" الإجابة: "+answer+suffix,SortOrder=q+1};
    int[] choices=[answer,answer-1,answer+1,answer+2];
    for(int c=0;c<4;c++){int source=(c+q)%4;question.Choices.Add(new ExamChoice{Id=question.Id+"-C"+(c+1),Text=choices[source]+suffix,IsCorrect=source==0,SortOrder=c+1});}
    exam.Questions.Add(question);
   }
   result.Add(exam);
  }
  return result;
 }
 public static ExamAttempt MakeAttempt(Exam exam,string student,int score,DateTime start,bool published) {
  var attempt=new ExamAttempt{Id=exam.Id+"-"+student,Exam=exam,ExamId=exam.Id,StudentId=student,StartedAtUtc=start,DeadlineAtUtc=start.AddMinutes(exam.DurationMinutes),SubmittedAtUtc=start.AddMinutes(3),PublishedAtUtc=published?start.AddHours(1):null};
  foreach(var question in exam.Questions.OrderBy(x=>x.SortOrder)) {
   bool correct=question.SortOrder<=score/2;var choice=question.Choices.First(x=>x.IsCorrect==correct);
   attempt.Answers.Add(new ExamAnswer{Id=attempt.Id+"-Q"+question.SortOrder,Question=question,QuestionId=question.Id,Choice=choice,ChoiceId=choice.Id});
  }
  attempt.Score=attempt.Answers.Where(x=>x.Choice.IsCorrect).Sum(x=>x.Question.Points);
  return attempt;
 }
 private static void AddMessages(ApplicationDbContext db,DateTime anchor) {
  var contexts=new[]{("C01","P01","S01",ConversationContext.Assignment,"ALG-W01"), ("C02","S01","S01",ConversationContext.Lesson,"ALG-L02"), ("C03","P01","S02",ConversationContext.Exam,"GEO-X03")};
  string[][] texts=[["أحمد لم ينه واجب المعادلات، نحتاج إرشادًا للبدء.","ابدأ بطرح 5 من الطرفين، ثم جرّب السؤال الثاني. أرفقت ملزمة للمراجعة.","شكرًا، هل نرسل خطوات الحل مكتوبة؟"],["لماذا نطرح 4 من الطرفين؟","حتى نحافظ على تساوي الطرفين ونصل إلى 2x = 10.","مثال آخر: في 3x + 6 = 21 نطرح 6 ثم نقسم على 3."],["أريد فهم الخطأ في اختبار ياسين الأخير.","السؤال الخامس عن مساحة مربع ضلعه 4؛ نضرب 4 × 4 فنحصل على 16 وحدة مربعة.","هذه صورة توضح خطوات الحل والمراجعة."]];
  for(int i=0;i<contexts.Length;i++) {
   var (id,other,student,type,context)=contexts[i];var conversation=new Conversation{Id=id,OtherUserId=other,StudentId=student,ContextType=type,ContextId=context};
   for(int m=0;m<3;m++) {
    var sender=m==0?other:m==1?"T01":i==0?other:"T01";
    var message=new Message{Id=id+"-M"+(m+1),SenderId=sender,Text=texts[i][m],SentAtUtc=anchor.AddHours(-6+i).AddMinutes(m*15),ReadAtUtc=m<2?anchor.AddHours(-6+i).AddMinutes(m*15+5):null};
    if(i==0 && m==1){message.AttachmentPath="Lessons/ALG-L02.pdf";message.AttachmentName="ملزمة المعادلات.pdf";message.AttachmentType="application/pdf";}
    if(i==2 && m==2){message.AttachmentPath="SeedAttachments/geometry-solution.png";message.AttachmentName="خطوات حل الهندسة.png";message.AttachmentType="image/png";}
    conversation.Messages.Add(message);
   }
   db.Conversations.Add(conversation);
  }
  db.Announcements.AddRange(new Announcement{Id="ANN-ALL",Title="أهلًا بك في دروسي",Body="جرّب الأدوار وتابع أثر التغييرات داخل تجربتك المستقلة.",PublishedAtUtc=anchor.AddDays(-1)},
   new Announcement{Id="ANN-ALG",GroupId="GROUP-ALG",Title="اختبار المعادلات متاح الآن",Body="خمسة أسئلة، وخمس دقائق للمحاولة.",TargetUrl="/Demo/Exam/ALG-X04",PublishedAtUtc=anchor.AddHours(-8)},
   new Announcement{Id="ANN-GEO",GroupId="GROUP-GEO",Title="تذكير بواجب المحيط والمساحة",Body="راجع الوحدات قبل تسليم الحل.",TargetUrl="/Demo/Assignment/GEO-W02",PublishedAtUtc=anchor.AddHours(-5)});
  var notifications=new[]{("P01","S01","غياب أحمد في آخر حصة معتمدة","/Demo/Session/A2"),("P01","S01","واجب أحمد يحتاج متابعة","/Demo/Assignment/ALG-W01"),("P01","S02","نتيجة ياسين الجديدة منشورة","/Demo/Exam/GEO-X03"),("P01","S02","اشتراك ياسين ينتهي قريبًا","/Demo/Dashboard?section=subscriptions"),("S01","S01","اختبار المعادلات متاح الآن","/Demo/Exam/ALG-X04"),("S02","S02","تصحيح واجبك منشور","/Demo/Assignment/GEO-W01"),("T01","S05","تسليم ينتظر التصحيح","/Demo/Assignment/ALG-W01?studentId=S05"),("T01","S01","رسالة جديدة من ولي الأمر","/Demo/Chat/C01")};
  for(int n=0;n<notifications.Length;n++){var (user,student,title,url)=notifications[n];db.Notifications.Add(new Notification{Id="NOT-"+(n+1),UserId=user,StudentId=student,Title=title,TargetUrl=url,EventKey="seed-"+(n+1),CreatedAtUtc=anchor.AddHours(-4).AddMinutes(n)});}
 }
}



