using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Microsoft.EntityFrameworkCore;
namespace Droosy_DAL.Seeding;
public class DemoSeeder(ApplicationDbContext db)
{
 public async Task SeedAsync()
 {
  var state = await db.DemoStates.SingleOrDefaultAsync();
  if (state != null)
  {
   if (state.SeedVersion != DemoSeedData.Version) throw new InvalidOperationException("Demo seed version changed. A deliberate data upgrade is required; existing changes were preserved.");
   return;
  }
  if (await db.Users.AnyAsync()) throw new InvalidOperationException("Demo data exists without a seed marker. Seeding stopped without deleting data.");
  var now = DateTime.UtcNow;
  var cairo = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
  var anchor = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(now, cairo));
  var midnight = TimeZoneInfo.ConvertTimeToUtc(anchor.ToDateTime(TimeOnly.MinValue), cairo);
  await using var transaction = await db.Database.BeginTransactionAsync();
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
  await db.SaveChangesAsync();
  if (await db.Users.CountAsync()!=10 || await db.Lessons.CountAsync()!=8 || await db.Subscriptions.SumAsync(x=>x.Price)!=2200 || await db.Payments.SumAsync(x=>x.Amount)!=1450) throw new InvalidOperationException("Demo seed validation failed.");
  await transaction.CommitAsync();
 }
}
