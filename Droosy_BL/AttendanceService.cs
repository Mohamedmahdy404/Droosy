using System.Text.Json;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Droosy_BL;

public record AttendanceEntry(string StudentId, AttendanceStatus Status, string Note);

public class AttendanceService(ApplicationDbContext db, NotificationService? notifications = null)
{
    public async Task<string?> SaveAsync(string sessionId, string teacherId, IReadOnlyList<AttendanceEntry> entries, bool approve)
    {
        var session = await db.ClassSessions.Include(x => x.Group).ThenInclude(x => x.Students)
            .Include(x => x.Attendances).SingleOrDefaultAsync(x => x.Id == sessionId);
        if (session == null) return "الحصة غير موجودة.";
        var error = ApplyRegister(session, teacherId, entries, approve, DateTime.UtcNow, out var audit);
        if (error != null) return error;
        if (audit != null)
        {
            db.AuditLogs.Add(audit);
            if(notifications!=null)
            {
                var old=JsonSerializer.Deserialize<List<AttendanceEntry>>(audit.BeforeJson)??[];
                foreach(var row in session.Attendances.Where(x=>!old.Any(o=>o.StudentId==x.StudentId&&o.Status==x.PublishedStatus&&o.Note==x.PublishedNote)))
                    await notifications.StudentAsync(row.StudentId,"تم اعتماد أو تحديث حضور الحصة","/Demo/Session/"+session.Id,"Attendance:"+audit.Id);
            }
        }
        // EF saves the complete register and its audit entry in one transaction.
        await db.SaveChangesAsync();
        return null;
    }

    public static string? ApplyRegister(ClassSession session, string teacherId, IReadOnlyList<AttendanceEntry> entries,
        bool approve, DateTime now, out AuditLog? audit)
    {
        audit = null;
        if (session.EndsAtUtc > now) return "يمكن تسجيل الحضور بعد انتهاء الحصة.";
        var members = session.Group.Students.Select(x => x.StudentId).Order().ToArray();
        if (!members.SequenceEqual(entries.Select(x => x.StudentId).Order()) ||
            entries.Any(x => !Enum.IsDefined(x.Status) || x.Note.Length > 300))
            return "اختر حالة صحيحة لكل طالب في المجموعة، وملاحظة لا تتجاوز ٣٠٠ حرف.";
        var before = Snapshot(session.Attendances, published: true);
        var wasApproved = session.IsApproved;
        foreach (var input in entries)
        {
            var row = session.Attendances.SingleOrDefault(x => x.StudentId == input.StudentId);
            if (row == null)
            {
                row = new Attendance { Id = session.Id + "-" + input.StudentId, StudentId = input.StudentId };
                session.Attendances.Add(row);
            }
            row.Status = input.Status; row.Note = input.Note.Trim();
            if (approve) { row.PublishedStatus = row.Status; row.PublishedNote = row.Note; }
        }
        session.DraftSavedAtUtc = now;
        var after = Snapshot(session.Attendances, published: true);
        if (approve && (!wasApproved || before != after))
        {
            session.IsApproved = true; session.ApprovedAtUtc = now;
            audit = new AuditLog
            {
                Id = Guid.NewGuid().ToString("N"), ActorId = teacherId, EntityType = nameof(ClassSession), EntityId = session.Id,
                Action = wasApproved ? "CorrectAttendance" : "ApproveAttendance", BeforeJson = before, AfterJson = after,
                CreatedAtUtc = now
            };
        }
        return null;
    }

    public static string Snapshot(IEnumerable<Attendance> rows, bool published) => JsonSerializer.Serialize(rows
        .Where(x => !published || x.PublishedStatus.HasValue).OrderBy(x => x.StudentId)
        .Select(x => new { x.StudentId, Status = published ? x.PublishedStatus : x.Status, Note = published ? x.PublishedNote : x.Note }));
}

