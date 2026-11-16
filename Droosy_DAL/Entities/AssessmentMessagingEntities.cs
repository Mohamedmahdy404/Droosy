namespace Droosy_DAL.Entities;

public enum SubmissionStatus { Draft, Submitted, Returned, Graded }
public enum ConversationContext { Lesson, Assignment, Exam }

public class Assignment : SeedEntity
{
    public string CourseId { get; set; } = "";
    public Course Course { get; set; } = null!;
    public string Title { get; set; } = "";
    public string Instructions { get; set; } = "";
    public DateTime DueAtUtc { get; set; }
    public int MaxScore { get; set; } = 10;
    public List<AssignmentSubmission> Submissions { get; set; } = [];
}
public class AssignmentSubmission : SeedEntity
{
    public string AssignmentId { get; set; } = "";
    public Assignment Assignment { get; set; } = null!;
    public string StudentId { get; set; } = "";
    public AppUser Student { get; set; } = null!;
    public string AnswerText { get; set; } = "";
    public string? AttachmentPath { get; set; }
    public string? AttachmentName { get; set; }
    public string? AttachmentType { get; set; }
    public SubmissionStatus Status { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public DateTime? GradedAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public int? Score { get; set; }
    public string Feedback { get; set; } = "";
    public int? PublishedScore { get; set; }
    public string PublishedFeedback { get; set; } = "";
    public string ReturnReason { get; set; } = "";
}
public class Exam : SeedEntity
{
    public string CourseId { get; set; } = "";
    public Course Course { get; set; } = null!;
    public string Title { get; set; } = "";
    public DateTime OpensAtUtc { get; set; }
    public DateTime ClosesAtUtc { get; set; }
    public int DurationMinutes { get; set; } = 5;
    public List<ExamQuestion> Questions { get; set; } = [];
    public List<ExamAttempt> Attempts { get; set; } = [];
}
public class ExamQuestion : SeedEntity
{
    public string ExamId { get; set; } = "";
    public Exam Exam { get; set; } = null!;
    public string Text { get; set; } = "";
    public string Explanation { get; set; } = "";
    public int SortOrder { get; set; }
    public int Points { get; set; } = 2;
    public List<ExamChoice> Choices { get; set; } = [];
}
public class ExamChoice : SeedEntity
{
    public string QuestionId { get; set; } = "";
    public ExamQuestion Question { get; set; } = null!;
    public string Text { get; set; } = "";
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}
public class ExamAttempt : SeedEntity
{
    public string ExamId { get; set; } = "";
    public Exam Exam { get; set; } = null!;
    public string StudentId { get; set; } = "";
    public AppUser Student { get; set; } = null!;
    public DateTime StartedAtUtc { get; set; }
    public DateTime DeadlineAtUtc { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public int? Score { get; set; }
    public List<ExamAnswer> Answers { get; set; } = [];
}
public class ExamAnswer : SeedEntity
{
    public string AttemptId { get; set; } = "";
    public ExamAttempt Attempt { get; set; } = null!;
    public string QuestionId { get; set; } = "";
    public ExamQuestion Question { get; set; } = null!;
    public string ChoiceId { get; set; } = "";
    public ExamChoice Choice { get; set; } = null!;
}
public class Conversation : SeedEntity
{
    public string TeacherId { get; set; } = "T01";
    public AppUser Teacher { get; set; } = null!;
    public string OtherUserId { get; set; } = "";
    public AppUser OtherUser { get; set; } = null!;
    public string StudentId { get; set; } = "";
    public AppUser Student { get; set; } = null!;
    public ConversationContext ContextType { get; set; }
    public string ContextId { get; set; } = "";
    public List<Message> Messages { get; set; } = [];
}
public class Message : SeedEntity
{
    public string ConversationId { get; set; } = "";
    public Conversation Conversation { get; set; } = null!;
    public string SenderId { get; set; } = "";
    public AppUser Sender { get; set; } = null!;
    public string Text { get; set; } = "";
    public DateTime SentAtUtc { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public string? AttachmentPath { get; set; }
    public string? AttachmentName { get; set; }
    public string? AttachmentType { get; set; }
}
public class Announcement : SeedEntity
{
    public string? GroupId { get; set; }
    public StudyGroup? Group { get; set; }
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string TargetUrl { get; set; } = "/Demo/Dashboard";
    public DateTime PublishedAtUtc { get; set; }
}
public class Notification : SeedEntity
{
    public string UserId { get; set; } = "";
    public AppUser User { get; set; } = null!;
    public string? StudentId { get; set; }
    public string Title { get; set; } = "";
    public string TargetUrl { get; set; } = "";
    public string EventKey { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ReadAtUtc { get; set; }
}


