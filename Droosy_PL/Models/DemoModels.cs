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
