using Droosy_PL.Models;
using Droosy_DAL.Entities;
public static class FeatureRenderCases
{
 public static List<(string Name,string View,object Model,string Expected)> Create()
 {
  var cases=new List<(string,string,object,string)>();
  foreach(var role in new[]{"Teacher","Student","Parent"})
  foreach(var view in new[]{"Assignments","Exams","Results","Messages","Announcements","Notifications","Weekly"})
   cases.Add(($"{role} {view}",view,FinalFixture.Create(role,view.ToLowerInvariant()),"<main"));
  foreach(var view in new[]{"Students","Review"})cases.Add(($"teacher {view}",view,FinalFixture.Create("Teacher"),"<main"));
  foreach(var role in new[]{"Teacher","Student","Parent"})
  {
   var p=FinalFixture.Create(role,"assignments");var work=p.Assignments.First();var row=p.Submissions.FirstOrDefault(x=>x.AssignmentId==work.Id&&x.StudentId=="S01");
   cases.Add(($"{role} assignment detail","Assignment",new AssignmentPageModel{Page=p,Assignment=work,Submission=row,SelectedStudent=p.Student,TeacherSubmissions=p.Submissions.Where(x=>x.AssignmentId==work.Id).ToList()},work.Title));
   p=FinalFixture.Create(role,"exams");var exam=p.Exams.First();var attempt=p.Attempts.SingleOrDefault(x=>x.ExamId==exam.Id&&x.StudentId=="S01");
   cases.Add(($"{role} exam review","Exam",new ExamPageModel{Page=p,Exam=exam,Attempt=attempt,SelectedStudent=p.Student,TeacherAttempts=p.Attempts.Where(x=>x.ExamId==exam.Id).ToList()},exam.Title));
   p=FinalFixture.Create(role,"messages");var chat=p.Conversations.First();
   cases.Add(($"{role} chat","Chat",new ChatPageModel{Page=p,Conversation=chat,ContextTitle="Context check"},"Context check"));
  }
  var solving=FinalFixture.Create("Student","exams","S05");var current=solving.Exams.Single(x=>x.Id=="ALG-X04");
  cases.Add(("ongoing timed exam","Exam",new ExamPageModel{Page=solving,Exam=current,Attempt=solving.Attempts.Single(x=>x.ExamId==current.Id),SelectedStudent=solving.Student},"data-exam-form"));
  var unpublished=FinalFixture.Create("Student","exams","S03");current=unpublished.Exams.Single(x=>x.Id=="ALG-X04");
  cases.Add(("unpublished exam","Exam",new ExamPageModel{Page=unpublished,Exam=current,Attempt=unpublished.Attempts.Single(x=>x.ExamId==current.Id),SelectedStudent=unpublished.Student},current.Title));
  cases.Add(("real zero results","Results",FinalFixture.Create("Parent","results","S02"),"GEO-X01"));
  return cases;
 }
}
