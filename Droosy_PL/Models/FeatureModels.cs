using System.ComponentModel.DataAnnotations;
using Droosy_DAL.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace Droosy_PL.Models;
public class AssignmentPageModel
{
 public DemoPage Page {get;set;}=null!;
 public Assignment Assignment {get;set;}=null!;
 public AssignmentSubmission? Submission {get;set;}
 public AppUser? SelectedStudent {get;set;}
 public List<AssignmentSubmission> TeacherSubmissions {get;set;}=[];
}
public class ExamPageModel
{
 public DemoPage Page {get;set;}=null!;
 public Exam Exam {get;set;}=null!;
 public ExamAttempt? Attempt {get;set;}
 public AppUser? SelectedStudent {get;set;}
 public List<ExamAttempt> TeacherAttempts {get;set;}=[];
 public bool CanStart {get;set;}
 public bool RevealAnswers=>Page.IsTeacher||Attempt?.PublishedAtUtc!=null;
}
public class ChatPageModel
{
 public DemoPage Page {get;set;}=null!;
 public Conversation Conversation {get;set;}=null!;
 public string ContextTitle {get;set;}="";
 public Guid ClientMessageId {get;set;}=Guid.NewGuid();
}
public record AttentionItem(string Title,string Detail,string Url,string Tone);
