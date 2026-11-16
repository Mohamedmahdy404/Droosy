using System.ComponentModel.DataAnnotations;
using Droosy_DAL.Entities;
namespace Droosy_PL.Models;
public class DemoPage {
 public AppUser? Account {get;set;}
 public AppUser? Student {get;set;}
 public List<AppUser> Students {get;set;}=[];
 public List<Course> Courses {get;set;}=[];
 public List<StudyGroup> Groups {get;set;}=[];
 public List<Subscription> Subscriptions {get;set;}=[];
 public List<ActivationCode> Codes {get;set;}=[];
 public List<ClassSession> Sessions {get;set;}=[];
 public List<LessonProgress> ProgressRows {get;set;}=[];
 public DateOnly? From {get;set;}
 public DateOnly? To {get;set;}
 public ClassSession? SelectedSession {get;set;}
 public LessonProgress? CurrentProgress {get;set;}
 public List<Assignment> Assignments {get;set;}=[];
 public List<AssignmentSubmission> Submissions {get;set;}=[];
 public List<Exam> Exams {get;set;}=[];
 public List<ExamAttempt> Attempts {get;set;}=[];
 public List<Conversation> Conversations {get;set;}=[];
 public List<Notification> Notifications {get;set;}=[];
 public List<Announcement> Announcements {get;set;}=[];
 public List<AttentionItem> Attention {get;set;}=[];
 public DateTime WeekFromUtc {get;set;}
 public DateTime WeekToUtc {get;set;}
 public int UnreadMessages=>Conversations.Sum(c=>c.Messages.Count(m=>m.SenderId!=Account?.Id&&m.ReadAtUtc==null));
 public int PendingAssignments=>Submissions.Count(s=>NeedsReview(s));
 public int PendingExams=>Attempts.Count(a=>a.SubmittedAtUtc!=null&&a.PublishedAtUtc==null);
 public static bool NeedsReview(AssignmentSubmission s)=>s.Status==SubmissionStatus.Submitted||(s.Status==SubmissionStatus.Graded&&(!s.PublishedAtUtc.HasValue||s.Score!=s.PublishedScore||s.Feedback!=s.PublishedFeedback));
 public Lesson? Lesson {get;set;}
 public bool CanOpen {get;set;}
 public string Section {get;set;}="home";
 public string Title {get;set;}="دروسي";
 public string Subtitle {get;set;}="تعلم ومتابعة في مكان واحد";
 public bool IsTeacher=>Account?.Role==DemoRole.Teacher;
 public bool IsParent=>Account?.Role==DemoRole.Parent;
 public static string Date(DateTime d)=>TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.SpecifyKind(d,DateTimeKind.Utc),TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")).ToString("dd / MM / yyyy");
 public static string DateAndTime(DateTime d)=>TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.SpecifyKind(d,DateTimeKind.Utc),TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")).ToString("dd / MM / yyyy · HH:mm");
}
public class EditLessonModel {
 public string Id {get;set;}="";
 public string CourseId {get;set;}="";
 [Required(ErrorMessage="اكتب عنوان الدرس"),StringLength(160,MinimumLength=3,ErrorMessage="العنوان من ٣ إلى ١٦٠ حرفًا")] public string Title {get;set;}="";
 [Required(ErrorMessage="اكتب وصف الدرس"),StringLength(2000)] public string Description {get;set;}="";
 public PublicationStatus Status {get;set;}
 public bool IsPreview {get;set;}
 public DateTime? PublishAtLocal {get;set;}
}

