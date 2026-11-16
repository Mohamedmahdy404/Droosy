using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Microsoft.EntityFrameworkCore;
namespace Droosy_DAL.Seeding;
public class DemoSeeder(ApplicationDbContext db)
{
 public async Task SeedAsync()
 {
  var state=await db.DemoStates.SingleOrDefaultAsync();
  if(state?.SeedVersion==DemoSeedData.Version)return;
  await using var transaction=db.Database.CurrentTransaction==null?await db.Database.BeginTransactionAsync():null;
  bool fresh=state==null;
  if(fresh)
  {
   if(await db.Users.AnyAsync())throw new InvalidOperationException("Data exists without a seed marker; nothing was deleted.");
   Populate(db,DateTime.UtcNow);
  }
  else
  {
   if(state!.SeedVersion is not ("droosy-demo-v1-phases-1-3" or "droosy-demo-v1-phases-1-5"))throw new InvalidOperationException("Unknown seed version; existing data was preserved.");
   var anchor=DateTime.SpecifyKind(state.SeededAtUtc,DateTimeKind.Utc);
   if(state.SeedVersion=="droosy-demo-v1-phases-1-3")DemoPhase45Seed.Add(db,anchor);
   DemoFeatureSeed.Add(db,anchor);state.SeedVersion=DemoSeedData.Version;
  }
  await db.SaveChangesAsync();
  if(fresh)await ValidateAsync(db);
  if(transaction!=null)await transaction.CommitAsync();
 }
 public static void Populate(ApplicationDbContext db,DateTime now)
 {
  var cairo=TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
  var anchor=DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(now,cairo));
  var midnight=TimeZoneInfo.ConvertTimeToUtc(anchor.ToDateTime(TimeOnly.MinValue),cairo);
  db.Users.AddRange(DemoSeedData.Users.Select(x => new AppUser { Id=x.Id, DisplayName=x.Name, Role=x.Role }));
  db.PlatformSettings.Add(new() { Id="PLATFORM" });
  db.AcademicLevels.AddRange(new AcademicLevel { Id="LEVEL-2", Name="الصف الثاني الإعدادي", SortOrder=2 }, new AcademicLevel { Id="LEVEL-1", Name="الصف الأول الإعدادي", SortOrder=1 });
  db.ParentStudents.AddRange(new ParentStudent { ParentId="P01", StudentId="S01" }, new ParentStudent { ParentId="P01", StudentId="S02" });
  db.Courses.AddRange(new Course { Id="ALG", Title="أساسيات الجبر", Description="خطوة بخطوة، نفهم المعادلات ونكتشف متعة حل المسائل.", LevelId="LEVEL-2", TeacherId="T01", MonthlyPrice=300 }, new Course { Id="GEO", Title="أساسيات الهندسة", Description="نفهم الزوايا والأشكال ونربط الرياضيات بالعالم من حولنا.", LevelId="LEVEL-1", TeacherId="T01", MonthlyPrice=250 });
  foreach (var content in DemoSeedData.Content)
  {
   for (int u=0; u<2; u++) db.CourseUnits.Add(new() { Id=content.Course+"-U"+(u+1), CourseId=content.Course, Title=content.Units[u], SortOrder=u+1 });
   for (int l=0; l<4; l++) db.Lessons.Add(new() { Id=content.Course+"-L0"+(l+1), UnitId=content.Course+"-U"+(l/2+1), Title=content.Lessons[l], Description=content.Descriptions[l], ContentType=l==1?LessonContentType.Video:LessonContentType.Text, IsPreview=l==0, SortOrder=l+1, PublicationStatus=l<3?PublicationStatus.Published:content.Course=="ALG"?PublicationStatus.Draft:PublicationStatus.Scheduled, PublishAtUtc=l==3&&content.Course=="GEO"?midnight.AddDays(3).AddHours(10):null });
  }
  db.StudyGroups.AddRange(new StudyGroup { Id="GROUP-ALG", CourseId="ALG", Name="مجموعة الجبر", Schedule="الأحد والثلاثاء • ٥ مساءً • حضوري" }, new StudyGroup { Id="GROUP-GEO", CourseId="GEO", Name="مجموعة الهندسة", Schedule="الاثنين والأربعاء • ٤ مساءً • حضوري" });
  foreach (var s in DemoSeedData.Subscriptions)
  {
   db.GroupStudents.Add(new() { GroupId="GROUP-"+s.Course, StudentId=s.Student });
   var end=midnight.AddDays(s.EndDays);
   db.Subscriptions.Add(new() { Id="SUB-"+s.Student, StudentId=s.Student, CourseId=s.Course, StartsAtUtc=end.AddDays(-30), EndsAtUtc=end, Price=s.Price });
   if (s.Paid>0) db.Payments.Add(new() { Id="PAY-"+s.Student, SubscriptionId="SUB-"+s.Student, Amount=s.Paid, Method=s.Student=="S03"?"كود تفعيل":"تحصيل تجريبي", PaidAtUtc=new DateTime(Math.Clamp(now.AddDays(-2).Ticks, end.AddDays(-30).AddHours(1).Ticks, end.AddSeconds(-1).Ticks),DateTimeKind.Utc), RecordedByUserId="T01" });
  }
  db.ActivationCodes.AddRange(new ActivationCode { Id="CODE-TRY", Code="DROOSY-TRY01", CourseId="ALG", Value=300, ExpiresAtUtc=midnight.AddDays(30) }, new ActivationCode { Id="CODE-OLD", Code="DROOSY-OLD01", CourseId="GEO", Value=250, ExpiresAtUtc=midnight.AddDays(-1) }, new ActivationCode { Id="CODE-USED", Code="DROOSY-USED01", CourseId="ALG", Value=300, ExpiresAtUtc=midnight.AddDays(30), RedeemedAtUtc=now.AddDays(-2), RedeemedByStudentId="S03" });
  db.DemoStates.Add(new() { Id="DEMO", SeedVersion=DemoSeedData.Version, SeededAtUtc=now, DemoAnchorDate=anchor, DataGenerationId=Guid.NewGuid() });
  DemoPhase45Seed.Add(db, now);
  DemoFeatureSeed.Add(db,now);
 }
 public static readonly IReadOnlyDictionary<string,int> ExpectedCounts=new Dictionary<string,int>
 {
  [nameof(AppUser)]=10,[nameof(ParentStudent)]=2,[nameof(PlatformSettings)]=1,[nameof(AcademicLevel)]=2,[nameof(Course)]=2,[nameof(CourseUnit)]=4,[nameof(Lesson)]=8,[nameof(StudyGroup)]=2,[nameof(GroupStudent)]=8,
  [nameof(ClassSession)]=8,[nameof(Attendance)]=24,[nameof(LessonProgress)]=16,[nameof(Assignment)]=4,[nameof(AssignmentSubmission)]=10,[nameof(Exam)]=8,[nameof(ExamQuestion)]=40,[nameof(ExamChoice)]=160,[nameof(ExamAttempt)]=25,[nameof(ExamAnswer)]=122,
  [nameof(Subscription)]=8,[nameof(Payment)]=6,[nameof(ActivationCode)]=3,[nameof(Conversation)]=3,[nameof(Message)]=9,[nameof(Notification)]=8,[nameof(Announcement)]=3,[nameof(AuditLog)]=4,[nameof(DemoState)]=1
 };
 public static async Task ValidateAsync(ApplicationDbContext db)
 {
  var counts=new Dictionary<string,IQueryable<int>>
  {
   [nameof(AppUser)]=db.Users.Select(x=>1),[nameof(ParentStudent)]=db.ParentStudents.Select(x=>1),[nameof(PlatformSettings)]=db.PlatformSettings.Select(x=>1),[nameof(AcademicLevel)]=db.AcademicLevels.Select(x=>1),[nameof(Course)]=db.Courses.Select(x=>1),[nameof(CourseUnit)]=db.CourseUnits.Select(x=>1),[nameof(Lesson)]=db.Lessons.Select(x=>1),[nameof(StudyGroup)]=db.StudyGroups.Select(x=>1),[nameof(GroupStudent)]=db.GroupStudents.Select(x=>1),
   [nameof(ClassSession)]=db.ClassSessions.Select(x=>1),[nameof(Attendance)]=db.Attendances.Select(x=>1),[nameof(LessonProgress)]=db.LessonProgresses.Select(x=>1),[nameof(Assignment)]=db.Assignments.Select(x=>1),[nameof(AssignmentSubmission)]=db.AssignmentSubmissions.Select(x=>1),[nameof(Exam)]=db.Exams.Select(x=>1),[nameof(ExamQuestion)]=db.ExamQuestions.Select(x=>1),[nameof(ExamChoice)]=db.ExamChoices.Select(x=>1),[nameof(ExamAttempt)]=db.ExamAttempts.Select(x=>1),[nameof(ExamAnswer)]=db.ExamAnswers.Select(x=>1),
   [nameof(Subscription)]=db.Subscriptions.Select(x=>1),[nameof(Payment)]=db.Payments.Select(x=>1),[nameof(ActivationCode)]=db.ActivationCodes.Select(x=>1),[nameof(Conversation)]=db.Conversations.Select(x=>1),[nameof(Message)]=db.Messages.Select(x=>1),[nameof(Notification)]=db.Notifications.Select(x=>1),[nameof(Announcement)]=db.Announcements.Select(x=>1),[nameof(AuditLog)]=db.AuditLogs.Select(x=>1),[nameof(DemoState)]=db.DemoStates.Select(x=>1)
  };
  foreach(var (name,query) in counts)if(await query.CountAsync()!=ExpectedCounts[name])throw new InvalidOperationException("Seed coverage failed: "+name);
 }
}
