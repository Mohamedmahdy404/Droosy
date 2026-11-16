using System.Reflection;
using Droosy_BL;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Droosy_DAL.Seeding;
using Microsoft.EntityFrameworkCore;
public static class FeatureChecks
{
 private static void Require(bool value,string message){if(!value)throw new Exception(message);}
 public static void Run()
 {
  var options=new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=localhost;Database=NoConnection;Integrated Security=True;TrustServerCertificate=True").Options;
  using var db=new ApplicationDbContext(options,new DemoScope{VisitorId=Guid.NewGuid().ToString("N"),RuntimeId=Guid.NewGuid()});
  var now=DateTime.UtcNow;DemoSeeder.Populate(db,now);
  var entries=db.ChangeTracker.Entries().ToList();
  foreach(var (name,count) in DemoSeeder.ExpectedCounts)Require(entries.Count(e=>e.Metadata.ClrType.Name==name)==count,"Seed count "+name);
  typeof(ApplicationDbContext).GetMethod("StampVisitor",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(db,null);db.ChangeTracker.DetectChanges();
  Require(db.ChangeTracker.Entries<IDemoRow>().All(x=>x.Entity.VisitorId==db.VisitorId),"Full graph visitor stamp");
  foreach(var e in db.ChangeTracker.Entries())foreach(var fk in e.Metadata.GetForeignKeys())Require(e.Property("VisitorId").CurrentValue?.ToString()==db.VisitorId,"Tenant FK stamp");
  var submissions=entries.Where(e=>e.Entity is AssignmentSubmission).Select(e=>(AssignmentSubmission)e.Entity).ToList();
  Require(!submissions.Any(x=>x.StudentId=="S01"&&x.AssignmentId=="ALG-W01"),"Ahmed fake submission");
  Require(submissions.Where(x=>x.Status==SubmissionStatus.Draft).All(x=>x.SubmittedAtUtc==null),"Draft counts as submission");
  Require(submissions.Where(x=>x.SubmittedAtUtc!=null).All(x=>x.SubmittedAtUtc<=now),"Future seeded submission");
  Require(submissions.Single(x=>x.StudentId=="S08"&&x.AssignmentId=="GEO-W01").PublishedScore==0,"Zero assignment score lost");
  var exams=entries.Where(e=>e.Entity is Exam).Select(e=>(Exam)e.Entity).ToList();var attempts=entries.Where(e=>e.Entity is ExamAttempt).Select(e=>(ExamAttempt)e.Entity).ToList();
  Require(exams.All(e=>e.Questions.Count==5&&e.Questions.All(q=>q.Choices.Count==4&&q.Choices.Count(c=>c.IsCorrect)==1)),"Question coverage");
  foreach(var a in attempts.Where(x=>x.SubmittedAtUtc!=null))Require(a.Score==AssessmentService.CalculateScore(a.Exam,a.Answers),"Seed grade differs from actual answers");
  Require(!attempts.Any(x=>x.StudentId=="S01"&&x.ExamId=="ALG-X03"),"Missing exam replaced with score");
  Require(attempts.Single(x=>x.StudentId=="S02"&&x.ExamId=="GEO-X01").Score==0,"Zero exam score lost");
  var exam=exams.Single(x=>x.Id=="ALG-X04");var test=DemoFeatureSeed.MakeAttempt(exam,"S01",6,now,false);
  Require(AssessmentService.CalculateScore(exam,test.Answers)==6,"Three of five must equal six");
  test.Score=1000;Require(AssessmentService.CalculateScore(exam,test.Answers)==6,"Client score influenced calculation");
  test.SubmittedAtUtc=null;test.DeadlineAtUtc=now.AddSeconds(1);Require(!AssessmentService.FinalizeAttempt(test,now,false),"Premature timeout");
  Require(AssessmentService.FinalizeAttempt(test,now.AddSeconds(2),false)&&test.Score==6,"Server deadline finalization");
  Require(!AssessmentService.FinalizeAttempt(test,now.AddSeconds(3),true),"Repeated submit changed attempt");
  var published=new AssignmentSubmission{Status=SubmissionStatus.Graded,SubmittedAtUtc=now.AddHours(-1),Score=9,PublishedScore=9,Feedback="old",PublishedFeedback="old",PublishedAtUtc=now.AddMinutes(-30)};
  Require(AssessmentService.ApplyGrade(published,10,0,"new","save",now,out var changed)==null&&!changed&&published.PublishedScore==9&&published.PublishedFeedback=="old","Unpublished draft leaked");
  Require(AssessmentService.ApplyGrade(published,10,0,"new","publish",now,out changed)==null&&changed&&published.PublishedScore==0,"Publishing zero failed");
  AssessmentService.ApplyGrade(published,10,0,"new","publish",now,out changed);Require(!changed,"Repeated grade publish created event");
  Require(AssessmentService.ApplyGrade(published,10,11,"bad","publish",now,out changed)!=null,"Out of range grade accepted");
  Require(AssessmentService.ApplyGrade(published,10,null,"أكمل الخطوات","return",now,out changed)==null&&published.Status==SubmissionStatus.Returned&&published.PublishedAtUtc==null,"Return failed");
  var notifications=entries.Where(x=>x.Entity is Notification).Select(x=>(Notification)x.Entity).ToList();Require(notifications.Count==8&&notifications.All(n=>n.TargetUrl.StartsWith("/Demo/")),"Seed notification targets");
  var messages=entries.Where(x=>x.Entity is Message).Select(x=>(Message)x.Entity).ToList();Require(messages.Count(m=>m.AttachmentPath!=null)==2,"Original attachments");Require(messages.All(m=>m.ReadAtUtc==null||m.ReadAtUtc>=m.SentAtUtc),"Read timestamp precedes send");
  var yassin=FinalFixture.Create("Parent","weekly","S02");Require(yassin.Attempts.Any(a=>a.ExamId=="GEO-X03"&&a.PublishedAtUtc>=now.AddDays(-7)&&a.Exam.OpensAtUtc<now.AddDays(-6)),"Weekly report should use publication date");
  Console.WriteLine("FEATURE_CHECKS_PASS: all 28 seed counts, visitor graph, five questions, 6/10 calculation, zero vs missing, deadline/idempotency, published grade snapshots, returns, messages, weekly publication dates");
 }
}
