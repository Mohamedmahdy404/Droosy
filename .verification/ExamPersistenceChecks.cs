using System.Reflection;
using Droosy_BL;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Droosy_DAL.Seeding;
using Microsoft.EntityFrameworkCore;

public sealed class ExamRecordingContext(DemoScope scope):ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=localhost;Database=NotUsed;Integrated Security=True").Options,scope)
{
 public List<(string Id,EntityState State)> Answers=[];
 public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,CancellationToken cancellationToken=default)
 {
  typeof(ApplicationDbContext).GetMethod("StampVisitor",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(this,null);
  Answers=ChangeTracker.Entries<ExamAnswer>().Where(x=>x.Entity.AttemptId=="ALG-X04-S01").Select(x=>(x.Entity.Id,x.State)).ToList();
  return Task.FromResult(1);
 }
}
public static class ExamPersistenceChecks
{
 public static async Task RunAsync()
 {
  var scope=new DemoScope{VisitorId=Guid.NewGuid().ToString("N")};using var db=new ExamRecordingContext(scope);
  DemoSeeder.Populate(db,DateTime.UtcNow);
  typeof(ApplicationDbContext).GetMethod("StampVisitor",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(db,null);
  foreach(var entry in db.ChangeTracker.Entries().ToList())entry.State=EntityState.Unchanged;
  var exam=db.Exams.Local.Single(x=>x.Id=="ALG-X04");
  var attempt=new ExamAttempt{VisitorId=scope.VisitorId,Id="ALG-X04-S01",ExamId=exam.Id,Exam=exam,StudentId="S01",StartedAtUtc=DateTime.UtcNow,DeadlineAtUtc=DateTime.UtcNow.AddMinutes(5)};
  db.ExamAttempts.Attach(attempt);
  db.Notifications.Add(new Notification{Id="test-notification",UserId="T01",EventKey="SubmitExam:"+attempt.Id});
  var selected=exam.Questions.ToDictionary(q=>q.Id,q=>q.Choices.Single(c=>c.IsCorrect).Id);
  var service=new AssessmentService(db,new NotificationService(db));
  var error=await service.SaveAnswersAsync(attempt,selected,true);
  Console.WriteLine("ANSWER_STATES: "+string.Join(",",db.Answers.Select(x=>x.State)));
  if(error!=null||attempt.SubmittedAtUtc==null||attempt.Score!=10||db.Answers.Count!=5||db.Answers.Any(x=>x.State!=EntityState.Added))throw new Exception("Exam answer insert tracking is invalid");
  if(attempt.Answers.Any(x=>x.VisitorId!=scope.VisitorId||x.AttemptId!=attempt.Id))throw new Exception("Exam answer scope or parent is invalid");
  await service.SaveAnswersAsync(attempt,new Dictionary<string,string>(),true);
  if(attempt.Answers.Count!=5)throw new Exception("Duplicate submission changed answers");
  Console.WriteLine("EXAM_PERSISTENCE_PASS: five answers tracked as inserts, same visitor/attempt, final score, duplicate submission stable (no SQL)");
 }
}
