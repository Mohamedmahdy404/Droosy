namespace Droosy_DAL.Entities;

public enum AttendanceStatus { Present, Absent, Late, Excused }

public class ClassSession : SeedEntity
{
    public string GroupId { get; set; } = "";
    public StudyGroup Group { get; set; } = null!;
    public string Title { get; set; } = "";
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    public DateTime? DraftSavedAtUtc { get; set; }
    public List<Attendance> Attendances { get; set; } = [];
}

public class Attendance : SeedEntity
{
    public string SessionId { get; set; } = "";
    public ClassSession Session { get; set; } = null!;
    public string StudentId { get; set; } = "";
    public AppUser Student { get; set; } = null!;
    public AttendanceStatus Status { get; set; }
    public string Note { get; set; } = "";
    public AttendanceStatus? PublishedStatus { get; set; }
    public string PublishedNote { get; set; } = "";
}

public class LessonProgress : SeedEntity
{
    public string StudentId { get; set; } = "";
    public AppUser Student { get; set; } = null!;
    public string LessonId { get; set; } = "";
    public Lesson Lesson { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public DateTime LastOpenedAtUtc { get; set; }
    public double LastPositionSeconds { get; set; }
    public string WatchedRangesJson { get; set; } = "[]";
}

public class AuditLog : SeedEntity
{
    public string ActorId { get; set; } = "";
    public AppUser Actor { get; set; } = null!;
    public string EntityType { get; set; } = "";
    public string EntityId { get; set; } = "";
    public string Action { get; set; } = "";
    public string BeforeJson { get; set; } = "null";
    public string AfterJson { get; set; } = "{}";
    public DateTime CreatedAtUtc { get; set; }
}
