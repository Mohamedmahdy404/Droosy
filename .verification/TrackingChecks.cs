using Droosy_BL;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Droosy_DAL.Seeding;
using Droosy_PL.Models;
using Microsoft.EntityFrameworkCore;

public static class TrackingChecks
{
    private static ApplicationDbContext NewDb() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=localhost;Database=NoConnectionUsed;Integrated Security=True;TrustServerCertificate=True").Options);
    public static void AddFixture(DemoPage page)
    {
        using var db=NewDb();
        DemoPhase45Seed.Add(db, DateTime.UtcNow);
        var sessions=db.ChangeTracker.Entries<ClassSession>().Select(x=>x.Entity).ToList();
        var progressRows=db.ChangeTracker.Entries<LessonProgress>().Select(x=>x.Entity).ToList();
        db.ChangeTracker.Clear();
        foreach(var session in sessions) session.Group=page.Groups.Single(x=>x.Id==session.GroupId);
        page.Sessions=sessions.Where(x=>page.IsTeacher || (x.IsApproved && x.Group.Students.Any(s=>s.StudentId==page.Student!.Id))).OrderByDescending(x=>x.StartsAtUtc).ToList();
        page.ProgressRows=progressRows.Where(x=>page.IsTeacher || x.StudentId==page.Student!.Id).ToList();
    }
    public static AttendanceEditorModel Editor(DemoPage page, string sessionId)
    {
        var session=page.Sessions.Single(x=>x.Id==sessionId);
        return new AttendanceEditorModel {SessionId=sessionId,Session=session,Entries=session.Group.Students.Select(x=>
        {
            var a=session.Attendances.SingleOrDefault(a=>a.StudentId==x.StudentId);
            return new AttendanceInput {StudentId=x.StudentId, StudentName=x.Student.DisplayName,Status=a?.Status,Note=a?.Note??""};
        }).ToList()};
    }
    private static void Require(bool condition,string message) {if(!condition)throw new Exception(message);}
    public static void Run()
    {
        using var db=NewDb(); var now=DateTime.UtcNow;DemoPhase45Seed.Add(db,now);
        var sessions=db.ChangeTracker.Entries<ClassSession>().Select(x=>x.Entity).ToList();
        var rows=db.ChangeTracker.Entries<Attendance>().Select(x=>x.Entity).ToList();
        var progress=db.ChangeTracker.Entries<LessonProgress>().Select(x=>x.Entity).ToList();
        Require(sessions.Count==8 && rows.Count==24 && progress.Count==16 && db.ChangeTracker.Entries<AuditLog>().Count()==2,"Seed coverage mismatch");
        Require(sessions.Where(x=>x.Id is "A4" or "B4").All(x=>x.Attendances.Count==0),"Future sessions have attendance");
        Require(!sessions.Single(x=>x.Id=="A3").IsApproved && sessions.Single(x=>x.Id=="A2").Attendances.Single(x=>x.StudentId=="S01").PublishedStatus==AttendanceStatus.Absent,"Initial Ahmed state incorrect");
        var session=sessions.Single(x=>x.Id=="A3");
        session.Group=new StudyGroup {Students=DemoSeedData.Subscriptions.Where(x=>x.Course=="ALG").Select(x=>new GroupStudent {StudentId=x.Student}).ToList()};
        var entries=session.Attendances.Select(x=>new AttendanceEntry(x.StudentId,x.StudentId=="S01"?AttendanceStatus.Late:x.Status,"ملاحظة")).ToList();
        Require(AttendanceService.ApplyRegister(session,"T01",entries,false,now,out var audit)==null,"Draft failed");
        Require(!session.IsApproved && audit==null && session.Attendances.All(x=>x.PublishedStatus==null),"Draft leaked to parents");
        Require(AttendanceService.ApplyRegister(session,"T01",entries,true,now,out audit)==null && session.IsApproved && audit?.Action=="ApproveAttendance","Approval failed");
        Require(session.Attendances.Single(x=>x.StudentId=="S01").PublishedStatus==AttendanceStatus.Late,"Ahmed late missing");
        var original=AttendanceService.Snapshot(session.Attendances,true);
        var correction=entries.Select(x=>x.StudentId=="S01"?x with {Status=AttendanceStatus.Present,Note="تم التصحيح"}:x).ToList();
        AttendanceService.ApplyRegister(session,"T01",correction,false,now,out audit);
        Require(AttendanceService.Snapshot(session.Attendances,true)==original && audit==null,"Correction draft leaked");
        AttendanceService.ApplyRegister(session,"T01",correction,true,now,out audit);
        Require(audit?.Action=="CorrectAttendance" && audit.BeforeJson==original && audit.AfterJson!=original,"Correction audit incorrect");
        AttendanceService.ApplyRegister(session,"T01",correction,true,now,out audit);Require(audit==null,"Unchanged approval duplicated audit");
        Require(AttendanceService.ApplyRegister(session,"T01",entries.Take(3).ToList(),true,now,out audit)!=null,"Incomplete register accepted");
        var future=sessions.Single(x=>x.Id=="A4");future.Group=session.Group;
        Require(AttendanceService.ApplyRegister(future,"T01",entries,true,now,out audit)!=null && future.Attendances.Count==0,"Future session mutated");
        var video=new LessonProgress();
        ProgressService.ApplyVideo(video,48,[new[]{40d,48d}],now);Require(!video.IsCompleted && ProgressService.WatchedSeconds(video)==8,"Seek completed video");
        ProgressService.ApplyVideo(video,5,[new[]{0d,5d},new[]{0d,5d}],now);Require(ProgressService.WatchedSeconds(video)==13,"Replay double counted");
        ProgressService.ApplyVideo(video,43.2,[new[]{0d,43.2}],now);Require(video.IsCompleted,"90 percent did not complete");
        var threshold=new LessonProgress();ProgressService.ApplyVideo(threshold,43.19,[new[]{0d,43.19}],now);Require(!threshold.IsCompleted,"Early completion");ProgressService.ApplyVideo(threshold,43.2,[new[]{0d,43.2}],now);Require(threshold.IsCompleted,"90 percent threshold wrong");
        var completedAt=video.CompletedAtUtc;ProgressService.ApplyVideo(video,0,[],now.AddMinutes(1));
        Require(video.IsCompleted && video.CompletedAtUtc==completedAt && ProgressService.WatchedSeconds(video)>=43.2,"Progress regressed");
        var clamped=ProgressService.MergeRanges([new[]{-10d,100d},new[]{double.NaN,8d}],48);Require(clamped.Count==1 && clamped[0][0]==0 && clamped[0][1]==48,"Invalid watch ranges not bounded");
        Console.WriteLine("TRACKING_CHECKS_PASSED: seed counts, future sessions, draft isolation, approval, correction audit, unchanged approval, full register, unique viewing, 90 percent, monotonic completion");
    }
}
