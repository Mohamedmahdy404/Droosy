using Droosy_BL;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Droosy_DAL.Seeding;
using Droosy_PL.Models;
using Microsoft.EntityFrameworkCore;
public static class FinalFixture
{
 public static DemoPage Create(string role="Teacher",string section="home",string student="S01")
 {
  using var db=new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=localhost;Database=NoConnectionUsed;Integrated Security=True;TrustServerCertificate=True").Options);
  DemoSeeder.Populate(db,DateTime.UtcNow);
  var users=db.ChangeTracker.Entries<AppUser>().Select(x=>x.Entity).OrderBy(x=>x.Id).ToList();var courses=db.ChangeTracker.Entries<Course>().Select(x=>x.Entity).ToList();var groups=db.ChangeTracker.Entries<StudyGroup>().Select(x=>x.Entity).ToList();
  var teacher=role=="Teacher";var parent=role=="Parent";var account=users.Single(x=>x.Id==(teacher?"T01":parent?"P01":student));var selected=users.Single(x=>x.Id==student);
  var memberships=groups.Where(g=>g.Students.Any(s=>s.StudentId==student)).ToList();
  var p=new DemoPage{Account=account,Student=selected,Students=teacher?users.Where(x=>x.Role==DemoRole.Student).ToList():parent?users.Where(x=>x.Id is "S01" or "S02").ToList():[selected],Courses=teacher?courses:courses.Where(c=>memberships.Any(g=>g.CourseId==c.Id)).ToList(),Groups=teacher?groups:memberships,Section=section,Title=section,Subtitle="رحلة تعليمية مترابطة"};
  p.Subscriptions=db.ChangeTracker.Entries<Subscription>().Select(x=>x.Entity).Where(x=>teacher||x.StudentId==student).ToList();p.Codes=db.ChangeTracker.Entries<ActivationCode>().Select(x=>x.Entity).ToList();
  p.Sessions=db.ChangeTracker.Entries<ClassSession>().Select(x=>x.Entity).Where(x=>teacher||(x.IsApproved&&memberships.Any(g=>g.Id==x.GroupId))).OrderByDescending(x=>x.StartsAtUtc).ToList();
  p.ProgressRows=db.ChangeTracker.Entries<LessonProgress>().Select(x=>x.Entity).Where(x=>teacher||x.StudentId==student).ToList();
  p.Assignments=db.ChangeTracker.Entries<Assignment>().Select(x=>x.Entity).Where(x=>p.Courses.Any(c=>c.Id==x.CourseId)).ToList();
  p.Submissions=db.ChangeTracker.Entries<AssignmentSubmission>().Select(x=>x.Entity).Where(x=>teacher?x.Status!=SubmissionStatus.Draft:x.StudentId==student).ToList();
  p.Exams=db.ChangeTracker.Entries<Exam>().Select(x=>x.Entity).Where(x=>p.Courses.Any(c=>c.Id==x.CourseId)).ToList();p.Attempts=db.ChangeTracker.Entries<ExamAttempt>().Select(x=>x.Entity).Where(x=>teacher||x.StudentId==student).ToList();
  p.Conversations=db.ChangeTracker.Entries<Conversation>().Select(x=>x.Entity).Where(x=>(x.TeacherId==account.Id||x.OtherUserId==account.Id)&&(!parent||x.StudentId==student)).ToList();
  p.Announcements=db.ChangeTracker.Entries<Announcement>().Select(x=>x.Entity).Where(x=>teacher||x.GroupId==null||memberships.Any(g=>g.Id==x.GroupId)).ToList();
  p.Notifications=db.ChangeTracker.Entries<Notification>().Select(x=>x.Entity).Where(x=>x.UserId==account.Id&&(!parent||x.StudentId==null||x.StudentId==student)).ToList();
  p.Attention=DemoControllerAttention(p);p.WeekToUtc=DateTime.UtcNow;p.WeekFromUtc=p.WeekToUtc.AddDays(-7);
  db.ChangeTracker.Clear();return p;
 }
 private static List<AttentionItem> DemoControllerAttention(DemoPage p)=>Droosy_PL.Controllers.DemoController.BuildAttention(p);
}
