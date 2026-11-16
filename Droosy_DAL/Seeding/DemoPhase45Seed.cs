using System.Text.Json;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;

namespace Droosy_DAL.Seeding;

public static class DemoPhase45Seed
{
    public static void Add(ApplicationDbContext db, DateTime anchorUtc)
    {
        var register = new[]
        {
            ("A1", "GROUP-ALG", -7, true, new[] { AttendanceStatus.Present, AttendanceStatus.Late, AttendanceStatus.Excused, AttendanceStatus.Absent }),
            ("A2", "GROUP-ALG", -4, true, new[] { AttendanceStatus.Absent, AttendanceStatus.Present, AttendanceStatus.Present, AttendanceStatus.Present }),
            ("A3", "GROUP-ALG", -1, false, new[] { AttendanceStatus.Present, AttendanceStatus.Absent, AttendanceStatus.Late, AttendanceStatus.Present }),
            ("A4", "GROUP-ALG", 1, false, Array.Empty<AttendanceStatus>()),
            ("B1", "GROUP-GEO", -6, true, new[] { AttendanceStatus.Present, AttendanceStatus.Present, AttendanceStatus.Absent, AttendanceStatus.Present }),
            ("B2", "GROUP-GEO", -3, true, new[] { AttendanceStatus.Present, AttendanceStatus.Late, AttendanceStatus.Present, AttendanceStatus.Present }),
            ("B3", "GROUP-GEO", 0, true, new[] { AttendanceStatus.Late, AttendanceStatus.Present, AttendanceStatus.Excused, AttendanceStatus.Present }),
            ("B4", "GROUP-GEO", 2, false, Array.Empty<AttendanceStatus>())
        };
        foreach (var (id, group, days, approved, statuses) in register)
        {
            var start = id == "B3" ? anchorUtc.AddHours(-2) : anchorUtc.AddDays(days);
            var session = new ClassSession
            {
                Id = id, GroupId = group, Title = group == "GROUP-ALG" ? "حصة الجبر · " + id : "حصة الهندسة · " + id,
                StartsAtUtc = start, EndsAtUtc = start.AddHours(1), IsApproved = approved,
                ApprovedAtUtc = approved ? start.AddHours(1) : null,
                DraftSavedAtUtc = statuses.Length > 0 ? start.AddHours(1) : null
            };
            var members = DemoSeedData.Subscriptions.Where(x => "GROUP-" + x.Course == group).Select(x => x.Student).ToArray();
            for (int i = 0; i < statuses.Length; i++) session.Attendances.Add(new Attendance
            {
                Id = id + "-" + members[i], StudentId = members[i], Status = statuses[i],
                PublishedStatus = approved ? statuses[i] : null
            });
            db.ClassSessions.Add(session);
            if (id is "A1" or "A2") db.AuditLogs.Add(new AuditLog
            {
                Id = "AUDIT-" + id, ActorId = "T01", EntityType = nameof(ClassSession), EntityId = id,
                Action = "ApproveAttendance", BeforeJson = "[]", CreatedAtUtc = start.AddHours(1),
                AfterJson = JsonSerializer.Serialize(session.Attendances.Select(x => new { x.StudentId, Status = x.PublishedStatus, Note = "" }))
            });
        }
        var progress = new[]
        {
            ("S01", "ALG-L01", true), ("S01", "ALG-L02", false),
            ("S02", "GEO-L01", true), ("S02", "GEO-L02", true), ("S02", "GEO-L03", false),
            ("S03", "ALG-L01", true), ("S03", "ALG-L02", true), ("S04", "ALG-L01", false),
            ("S05", "ALG-L01", true), ("S05", "ALG-L02", false),
            ("S06", "GEO-L01", true), ("S06", "GEO-L02", true),
            ("S07", "GEO-L01", true), ("S07", "GEO-L03", false),
            ("S08", "GEO-L01", true), ("S08", "GEO-L02", false)
        };
        foreach (var (student, lesson, completed) in progress)
        {
            var video = lesson.EndsWith("L02");
            var position = video ? DemoLessonContent.VideoDurationSeconds * (completed ? 1 : .35) : 0;
            db.LessonProgresses.Add(new LessonProgress
            {
                Id = student + "-" + lesson, StudentId = student, LessonId = lesson, IsCompleted = completed,
                LastOpenedAtUtc = anchorUtc.AddHours(-3), CompletedAtUtc = completed ? anchorUtc.AddDays(-2) : null,
                LastPositionSeconds = position, WatchedRangesJson = video ? JsonSerializer.Serialize(new[] { new[] { 0d, position } }) : "[]"
            });
        }
    }
}
