using System.Text.Json;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Microsoft.EntityFrameworkCore;
namespace Droosy_BL;

public class AssessmentService(ApplicationDbContext db,NotificationService notifications)
{
 public static string SubmissionLabel(Assignment assignment,AssignmentSubmission? row,DateTime now)=>row==null||row.Status==SubmissionStatus.Draft?(assignment.DueAtUtc<now?"متأخر · لم يسلّم":"لم يسلّم بعد"):row.Status==SubmissionStatus.Returned?"معاد للتعديل":row.PublishedAtUtc.HasValue?"التصحيح منشور":row.Status==SubmissionStatus.Graded?"بانتظار نشر التصحيح":"بانتظار التصحيح";
 public static int CalculateScore(Exam exam,IEnumerable<ExamAnswer> answers)
 {
  var map=answers.GroupBy(x=>x.QuestionId).ToDictionary(x=>x.Key,x=>x.Last().ChoiceId);
  return exam.Questions.Where(q=>map.TryGetValue(q.Id,out var choice)&&q.Choices.Any(c=>c.Id==choice&&c.IsCorrect)).Sum(q=>q.Points);
 }
 public static bool FinalizeAttempt(ExamAttempt attempt,DateTime now,bool submit)
 {
  if(attempt.SubmittedAtUtc.HasValue||(!submit&&now<attempt.DeadlineAtUtc))return false;
  attempt.Score=CalculateScore(attempt.Exam,attempt.Answers);
  attempt.SubmittedAtUtc=now>=attempt.DeadlineAtUtc?attempt.DeadlineAtUtc:now;
  return true;
 }
 public async Task<ExamAttempt> StartAsync(Exam exam,string student)
 {
  var existing=await db.ExamAttempts.Include(x=>x.Answers).SingleOrDefaultAsync(x=>x.ExamId==exam.Id&&x.StudentId==student);
  if(existing!=null)return existing;
  var now=DateTime.UtcNow;
  if(now<exam.OpensAtUtc||now>=exam.ClosesAtUtc)throw new ArgumentException("الاختبار غير متاح للبدء الآن.");
  var attempt=new ExamAttempt{Id=exam.Id+"-"+student,ExamId=exam.Id,StudentId=student,StartedAtUtc=now,DeadlineAtUtc=new[]{exam.ClosesAtUtc,now.AddMinutes(exam.DurationMinutes)}.Min()};
  db.ExamAttempts.Add(attempt);await db.SaveChangesAsync();return attempt;
 }
 public async Task<string?> SaveAnswersAsync(ExamAttempt attempt,IReadOnlyDictionary<string,string> selected,bool submit)
 {
  var now=DateTime.UtcNow;
  if(attempt.SubmittedAtUtc.HasValue)return null;
  if(now>=attempt.DeadlineAtUtc){if(FinalizeAttempt(attempt,now,false))await notifications.AddAsync("T01",attempt.StudentId,"محاولة اختبار تنتظر المراجعة","/Demo/Exam/"+attempt.ExamId+"?studentId="+attempt.StudentId,"SubmitExam:"+attempt.Id);await db.SaveChangesAsync();return null;}
  foreach(var (questionId,choiceId) in selected)
  {
   var question=attempt.Exam.Questions.SingleOrDefault(x=>x.Id==questionId);
   if(question==null||!question.Choices.Any(x=>x.Id==choiceId))return "توجد إجابة غير مرتبطة بسؤال الاختبار.";
  }
  foreach(var (questionId,choiceId) in selected)
  {
   var row=attempt.Answers.SingleOrDefault(x=>x.QuestionId==questionId);
   if(row==null){row=new ExamAnswer{Id=attempt.Id+"-"+questionId,QuestionId=questionId};attempt.Answers.Add(row);}
   row.ChoiceId=choiceId;
  }
  if(FinalizeAttempt(attempt,now,submit))await notifications.AddAsync("T01",attempt.StudentId,"محاولة اختبار تنتظر المراجعة","/Demo/Exam/"+attempt.ExamId+"?studentId="+attempt.StudentId,"SubmitExam:"+attempt.Id);await db.SaveChangesAsync();return null;
 }
 public static string? ApplyGrade(AssignmentSubmission row,int maximum,int? score,string feedback,string intent,DateTime now,out bool changed)
 {
  changed=false;
  if(row.Status==SubmissionStatus.Draft||!row.SubmittedAtUtc.HasValue)return "لا يمكن تصحيح مسودة لم تُسلّم.";
  if(intent=="return")
  {
   if(string.IsNullOrWhiteSpace(feedback))return "اكتب سبب الإرجاع ليساعد الطالب على تعديل الحل.";
   if(row.Status==SubmissionStatus.Returned&&row.ReturnReason==feedback.Trim()&&row.PublishedAtUtc==null)return null;
   row.Status=SubmissionStatus.Returned;row.ReturnReason=feedback.Trim();row.Score=null;row.Feedback="";row.GradedAtUtc=null;row.PublishedAtUtc=null;row.PublishedScore=null;row.PublishedFeedback="";changed=true;return null;
  }
  if(intent is not ("save" or "publish")||score==null||score<0||score>maximum)return "أدخل درجة صحيحة من صفر إلى الدرجة النهائية.";
  row.Score=score;row.Feedback=feedback.Trim();row.Status=SubmissionStatus.Graded;row.GradedAtUtc=now;
  if(intent=="publish"&&(!row.PublishedAtUtc.HasValue||row.PublishedScore!=score||row.PublishedFeedback!=row.Feedback))
  {row.PublishedScore=score;row.PublishedFeedback=row.Feedback;row.PublishedAtUtc=now;changed=true;}
  return null;
 }
 public async Task<string?> GradeAsync(AssignmentSubmission row,int maximum,int? score,string feedback,string intent,string teacher)
 {
  var before=JsonSerializer.Serialize(new{Score=row.PublishedScore,row.PublishedFeedback,row.PublishedAtUtc});
  var error=ApplyGrade(row,maximum,score,feedback,intent,DateTime.UtcNow,out var changed);if(error!=null)return error;
  if(changed)
  {
   var action=intent=="return"?"ReturnAssignment":"PublishAssignment";
   db.AuditLogs.Add(new AuditLog{Id=Guid.NewGuid().ToString("N"),ActorId=teacher,EntityType=nameof(AssignmentSubmission),EntityId=row.Id,Action=action,BeforeJson=before,AfterJson=JsonSerializer.Serialize(new{Score=row.PublishedScore,row.PublishedFeedback,row.PublishedAtUtc,row.ReturnReason}),CreatedAtUtc=DateTime.UtcNow});
   await notifications.StudentAsync(row.StudentId,intent=="return"?"واجبك معاد للتعديل":"تصحيح واجبك منشور","/Demo/Assignment/"+row.AssignmentId,action+":"+row.Id+":"+DateTime.UtcNow.Ticks);
  }
  await db.SaveChangesAsync();return null;
 }
 public async Task PublishExamAsync(ExamAttempt attempt,string teacher)
 {
  if(!attempt.SubmittedAtUtc.HasValue)throw new ArgumentException("المحاولة لم تُسلّم بعد.");
  if(attempt.PublishedAtUtc.HasValue)return;
  attempt.Score=CalculateScore(attempt.Exam,attempt.Answers);attempt.PublishedAtUtc=DateTime.UtcNow;
  db.AuditLogs.Add(new AuditLog{Id=Guid.NewGuid().ToString("N"),ActorId=teacher,EntityType=nameof(ExamAttempt),EntityId=attempt.Id,Action="PublishExam",BeforeJson="{}",AfterJson=JsonSerializer.Serialize(new{attempt.Score,attempt.PublishedAtUtc}),CreatedAtUtc=DateTime.UtcNow});
  await notifications.StudentAsync(attempt.StudentId,"نتيجة اختبارك منشورة","/Demo/Exam/"+attempt.ExamId,"PublishExam:"+attempt.Id);
  await db.SaveChangesAsync();
 }
}


