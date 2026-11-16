namespace Droosy_DAL.Entities;

public enum DemoRole { Teacher, Student, Parent }
public enum PublicationStatus { Draft, Published, Scheduled }
public enum LessonContentType { Text, Video }
public interface IDemoRow { string VisitorId { get; set; } }
public abstract class SeedEntity : IDemoRow { public string VisitorId { get; set; } = "template"; public string Id { get; set; } = ""; }
public class AppUser : SeedEntity { public string DisplayName { get; set; } = ""; public DemoRole Role { get; set; } }
public class ParentStudent : IDemoRow { public string VisitorId { get; set; } = "template"; public string ParentId { get; set; } = ""; public AppUser Parent { get; set; } = null!; public string StudentId { get; set; } = ""; public AppUser Student { get; set; } = null!; }
public class PlatformSettings : SeedEntity { public string Name { get; set; } = "دروسي"; public string TeacherName { get; set; } = "أ. محمد عادل"; }
public class AcademicLevel : SeedEntity { public string Name { get; set; } = ""; public int SortOrder { get; set; } }
public class Course : SeedEntity {
 public string Title { get; set; } = ""; public string Description { get; set; } = "";
 public string LevelId { get; set; } = ""; public AcademicLevel Level { get; set; } = null!;
 public string TeacherId { get; set; } = ""; public AppUser Teacher { get; set; } = null!;
 public decimal MonthlyPrice { get; set; }
 public List<CourseUnit> Units { get; set; } = [];
}
public class CourseUnit : SeedEntity { public string CourseId { get; set; } = ""; public Course Course { get; set; } = null!; public string Title { get; set; } = ""; public int SortOrder { get; set; } public List<Lesson> Lessons { get; set; } = []; }
public class Lesson : SeedEntity {
 public string UnitId { get; set; } = ""; public CourseUnit Unit { get; set; } = null!;
 public string Title { get; set; } = ""; public string Description { get; set; } = "";
 public LessonContentType ContentType { get; set; }
 public bool IsPreview { get; set; } public PublicationStatus PublicationStatus { get; set; }
 public DateTime? PublishAtUtc { get; set; } public int SortOrder { get; set; }
}
public class StudyGroup : SeedEntity { public string CourseId { get; set; } = ""; public Course Course { get; set; } = null!; public string Name { get; set; } = ""; public string Schedule { get; set; } = ""; public List<GroupStudent> Students { get; set; } = []; }
public class GroupStudent : IDemoRow { public string VisitorId { get; set; } = "template"; public string GroupId { get; set; } = ""; public StudyGroup Group { get; set; } = null!; public string StudentId { get; set; } = ""; public AppUser Student { get; set; } = null!; }
public class Subscription : SeedEntity {
 public string StudentId { get; set; } = ""; public AppUser Student { get; set; } = null!;
 public string CourseId { get; set; } = ""; public Course Course { get; set; } = null!;
 public DateTime StartsAtUtc { get; set; } public DateTime EndsAtUtc { get; set; } public decimal Price { get; set; }
 public List<Payment> Payments { get; set; } = [];
}
public class Payment : SeedEntity { public string SubscriptionId { get; set; } = ""; public Subscription Subscription { get; set; } = null!; public decimal Amount { get; set; } public string Method { get; set; } = ""; public DateTime PaidAtUtc { get; set; } public string RecordedByUserId { get; set; } = ""; public AppUser RecordedByUser { get; set; } = null!; }
public class ActivationCode : SeedEntity { public string Code { get; set; } = ""; public string CourseId { get; set; } = ""; public Course Course { get; set; } = null!; public decimal Value { get; set; } public DateTime ExpiresAtUtc { get; set; } public DateTime? RedeemedAtUtc { get; set; } public string? RedeemedByStudentId { get; set; } public AppUser? RedeemedByStudent { get; set; } }
public class DemoState : SeedEntity { public Guid RuntimeId { get; set; } public string SeedVersion { get; set; } = ""; public DateTime SeededAtUtc { get; set; } public DateOnly DemoAnchorDate { get; set; } public Guid DataGenerationId { get; set; } }

