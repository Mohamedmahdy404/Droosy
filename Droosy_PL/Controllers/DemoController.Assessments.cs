using Droosy_BL;
using Droosy_DAL.Entities;
using Droosy_PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Droosy_PL.Controllers;

public partial class DemoController
{
 public async Task<IActionResult> Assignments(bool pending=false)
 {
  var page=await Page("assignments");if(page.Account==null)return RedirectToAction(nameof(Index));
  if(pending&&!page.IsTeacher)page.Assignments=page.Assignments.Where(a=>!page.Submissions.Any(s=>s.AssignmentId==a.Id&&s.StudentId==page.Student!.Id&&s.Status is not (SubmissionStatus.Draft or SubmissionStatus.Returned))).ToList();
  page.Title=page.IsTeacher?"الواجبات والتصحيح":"الواجبات";page.Subtitle="من المحاولة الأولى، لحد المراجعة والتصحيح";return View(page);
 }
 public async Task<IActionResult> Assignment(string id,string? studentId)
 {
  var page=await Page("assignments");if(page.Account==null)return RedirectToAction(nameof(Index));
  var assignment=page.Assignments.SingleOrDefault(x=>x.Id==id);if(assignment==null)return NotFound();
  var model=new AssignmentPageModel{Page=page,Assignment=assignment,SelectedStudent=page.Student,TeacherSubmissions=page.Submissions.Where(x=>x.AssignmentId==id&&x.Status!=SubmissionStatus.Draft).ToList()};
  if(page.IsTeacher&&studentId!=null)model.SelectedStudent=page.Students.SingleOrDefault(x=>x.Id==studentId);
  if(model.SelectedStudent!=null)model.Submission=page.Submissions.SingleOrDefault(x=>x.AssignmentId==id&&x.StudentId==model.SelectedStudent.Id);
  ViewData["Page"]=page;ViewData["Title"]=assignment.Title;return View(model);
 }
 [HttpPost,RequestSizeLimit(6*1024*1024)]
 public async Task<IActionResult> SaveAssignment(string id,string? answerText,string intent,IFormFile? attachment)
 {
  var page=await Page("assignments");if(page.Account==null||page.IsTeacher||page.IsParent)return BadRequest();
  var assignment=page.Assignments.SingleOrDefault(x=>x.Id==id);if(assignment==null)return NotFound();
  var row=await db.AssignmentSubmissions.SingleOrDefaultAsync(x=>x.AssignmentId==id&&x.StudentId==page.Account.Id);
  if(row!=null&&row.Status is not (SubmissionStatus.Draft or SubmissionStatus.Returned))return RedirectToAction(nameof(Assignment),new{id});
  if(intent is not ("draft" or "submit"))return BadRequest();
  if((answerText?.Length??0)>5000)return AssignmentError(id,"الحل لا يتجاوز ٥٠٠٠ حرف.");
  if(intent=="submit"&&string.IsNullOrWhiteSpace(answerText)&&attachment==null&&row?.AttachmentPath==null)return AssignmentError(id,"اكتب حلًا أو أرفق صورة أو PDF قبل التسليم.");
  Droosy_PL.Services.SavedUpload? file;
  try{file=await uploads.SaveAsync(attachment);}catch(ArgumentException e){return AssignmentError(id,e.Message);}
  if(intent=="submit"&&string.IsNullOrWhiteSpace(answerText)&&file==null&&row?.AttachmentPath==null)return AssignmentError(id,"اكتب حلًا أو اختر مرفقًا غير فارغ.");
  if(row==null){row=new AssignmentSubmission{Id=id+"-"+page.Account.Id,AssignmentId=id,StudentId=page.Account.Id};db.AssignmentSubmissions.Add(row);}
  var old=row.AttachmentPath;
  row.AnswerText=answerText?.Trim()??"";
  if(file!=null){row.AttachmentPath=file.Path;row.AttachmentName=file.Name;row.AttachmentType=file.ContentType;}
  if(intent=="submit")
  {
   row.Status=SubmissionStatus.Submitted;row.SubmittedAtUtc=DateTime.UtcNow;row.ReturnReason="";row.Score=null;row.GradedAtUtc=null;row.PublishedAtUtc=null;row.PublishedScore=null;row.PublishedFeedback="";row.Feedback="";
   await notifications.AddAsync("T01",page.Account.Id,"تسليم واجب جديد ينتظر التصحيح","/Demo/Assignment/"+id+"?studentId="+page.Account.Id,"SubmitAssignment:"+row.Id+":"+row.SubmittedAtUtc.Value.Ticks);
  }
  else {row.Status=SubmissionStatus.Draft;row.SubmittedAtUtc=null;}
  try{await db.SaveChangesAsync();}catch{if(file!=null)uploads.Delete(file.Path);throw;}
  if(file!=null)uploads.Delete(old);
  TempData["Success"]=intent=="submit"?"تم تسليم الواجب. سيظهر التصحيح بعد نشره.":"تم حفظ مسودتك؛ لم تُرسل للمدرس بعد.";
  return RedirectToAction(nameof(Assignment),new{id});
 }
 private IActionResult AssignmentError(string id,string message){TempData["Error"]=message;return RedirectToAction(nameof(Assignment),new{id});}
 [HttpPost]
 public async Task<IActionResult> GradeAssignment(string id,int? score,string? feedback,string intent)
 {
  var page=await Page("assignments");if(!page.IsTeacher)return BadRequest();
  var row=await db.AssignmentSubmissions.Include(x=>x.Assignment).SingleOrDefaultAsync(x=>x.Id==id);if(row==null)return NotFound();
  var error=await assessments.GradeAsync(row,row.Assignment.MaxScore,score,(feedback??"")[..Math.Min(feedback?.Length??0,2000)],intent,page.Account!.Id);
  if(error!=null)TempData["Error"]=error;else TempData["Success"]=intent=="publish"?"تم نشر التصحيح للطالب وولي الأمر.":intent=="return"?"تم إرجاع الحل للطالب مع سبب التعديل.":"تم حفظ التصحيح؛ النسخة المنشورة لم تتغير.";
  return RedirectToAction(nameof(Assignment),new{id=row.AssignmentId,studentId=row.StudentId});
 }
 public async Task<IActionResult> SubmissionAttachment(string id)
 {
  var page=await Page();if(page.Account==null)return RedirectToAction(nameof(Index));
  var row=await db.AssignmentSubmissions.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id);if(row?.AttachmentPath==null)return NotFound();
  if(page.IsTeacher?row.Status==SubmissionStatus.Draft:row.StudentId!=page.Student?.Id||(page.IsParent&&row.Status==SubmissionStatus.Draft))return NotFound();
  var path=uploads.Resolve(row.AttachmentPath);if(!System.IO.File.Exists(path))return NotFound();return PhysicalFile(path,row.AttachmentType??"application/octet-stream",row.AttachmentName,enableRangeProcessing:true);
 }
 public async Task<IActionResult> Exams()
 {
  var page=await Page("exams");if(page.Account==null)return RedirectToAction(nameof(Index));
  await ExpireAttempts(page);
  page.Title="الاختبارات";page.Subtitle="محاولة واحدة، ونتيجة واضحة بعد النشر";return View(page);
 }
 private async Task ExpireAttempts(DemoPage page)
 {
  var pending=await db.ExamAttempts.Include(x=>x.Answers).Where(x=>x.SubmittedAtUtc==null&&x.DeadlineAtUtc<=DateTime.UtcNow&&(page.IsTeacher||x.StudentId==page.Student!.Id)).ToListAsync();
  foreach(var attempt in pending)
  {
   attempt.Exam=await db.Exams.Include(x=>x.Questions).ThenInclude(x=>x.Choices).SingleAsync(x=>x.Id==attempt.ExamId);
   if(AssessmentService.FinalizeAttempt(attempt,DateTime.UtcNow,false))await notifications.AddAsync("T01",attempt.StudentId,"محاولة اختبار تنتظر نشر النتيجة","/Demo/Exam/"+attempt.ExamId+"?studentId="+attempt.StudentId,"SubmitExam:"+attempt.Id);
   var display=page.Attempts.SingleOrDefault(x=>x.Id==attempt.Id);if(display!=null){display.Score=attempt.Score;display.SubmittedAtUtc=attempt.SubmittedAtUtc;}
  }
  if(pending.Count>0)await db.SaveChangesAsync();
 }
 public async Task<IActionResult> Exam(string id,string? studentId)
 {
  var page=await Page("exams");if(page.Account==null)return RedirectToAction(nameof(Index));
  var exam=page.Exams.SingleOrDefault(x=>x.Id==id);if(exam==null)return NotFound();
  await ExpireAttempts(page);
  var student=page.IsTeacher&&studentId!=null?page.Students.SingleOrDefault(x=>x.Id==studentId):page.Student;
  var attempt=student==null?null:page.Attempts.SingleOrDefault(x=>x.ExamId==id&&x.StudentId==student.Id);
  var model=new ExamPageModel{Page=page,Exam=exam,Attempt=attempt,SelectedStudent=student,TeacherAttempts=page.Attempts.Where(x=>x.ExamId==id).ToList(),CanStart=!page.IsTeacher&&!page.IsParent&&attempt==null&&exam.OpensAtUtc<=DateTime.UtcNow&&exam.ClosesAtUtc>DateTime.UtcNow};
  ViewData["Page"]=page;ViewData["Title"]=exam.Title;return View(model);
 }
 [HttpPost]
 public async Task<IActionResult> StartExam(string id)
 {
  var page=await Page();if(page.IsTeacher||page.IsParent||page.Account==null)return BadRequest();
  var exam=page.Exams.SingleOrDefault(x=>x.Id==id);if(exam==null)return NotFound();
  try{await assessments.StartAsync(exam,page.Account.Id);}catch(ArgumentException e){TempData["Error"]=e.Message;}
  return RedirectToAction(nameof(Exam),new{id});
 }
 [HttpPost]
 public async Task<IActionResult> SaveExam(string id,string intent)
 {
  if(intent is not ("save" or "submit"))return BadRequest();
  var page=await Page();if(page.IsTeacher||page.IsParent||page.Account==null)return BadRequest();
  var attempt=await db.ExamAttempts.Include(x=>x.Answers).Include(x=>x.Exam).ThenInclude(x=>x.Questions).ThenInclude(x=>x.Choices).SingleOrDefaultAsync(x=>x.ExamId==id&&x.StudentId==page.Account.Id);
  if(attempt==null)return NotFound();
  var selected=new Dictionary<string,string>();
  foreach(var question in attempt.Exam.Questions){var value=Request.Form["answers["+question.Id+"]"].FirstOrDefault();if(!string.IsNullOrEmpty(value))selected[question.Id]=value;}
  var error=await assessments.SaveAnswersAsync(attempt,selected,intent=="submit");
  if(Request.Headers.Accept.Any(x=>x?.Contains("application/json")==true))return error==null?Json(new{submitted=attempt.SubmittedAtUtc!=null,serverNowUtc=DateTime.UtcNow,deadlineUtc=DateTime.SpecifyKind(attempt.DeadlineAtUtc,DateTimeKind.Utc)}):BadRequest(new{error});
  if(error!=null)TempData["Error"]=error;else TempData["Success"]=attempt.SubmittedAtUtc!=null?"تم تسليم الاختبار. النتيجة ستظهر بعد نشرها.":"تم حفظ الإجابات؛ الوقت لا يزال محسوبًا من بداية المحاولة.";
  return RedirectToAction(nameof(Exam),new{id});
 }
 [HttpPost]
 public async Task<IActionResult> PublishExam(string id)
 {
  var page=await Page();if(!page.IsTeacher)return BadRequest();
  var attempt=await db.ExamAttempts.Include(x=>x.Answers).Include(x=>x.Exam).ThenInclude(x=>x.Questions).ThenInclude(x=>x.Choices).SingleOrDefaultAsync(x=>x.Id==id);if(attempt==null)return NotFound();
  try{await assessments.PublishExamAsync(attempt,page.Account!.Id);TempData["Success"]="تم نشر النتيجة مع مراجعة الإجابات.";}catch(ArgumentException e){TempData["Error"]=e.Message;}
  return RedirectToAction(nameof(Exam),new{id=attempt.ExamId,studentId=attempt.StudentId});
 }
 public async Task<IActionResult> Results(DateOnly? from,DateOnly? to,string? studentId)
 {
  var page=await Page("results");if(page.Account==null)return RedirectToAction(nameof(Index));await ExpireAttempts(page);
  if(page.IsTeacher){page.Student=page.Students.SingleOrDefault(x=>x.Id==studentId)??page.Students.First();page.Courses=await learning.CoursesForAsync(page.Student.Id);var courses=page.Courses.Select(x=>x.Id).ToArray();page.Exams=page.Exams.Where(x=>courses.Contains(x.CourseId)).ToList();page.Attempts=page.Attempts.Where(x=>x.StudentId==page.Student.Id).ToList();}
  page.From=from;page.To=to;page.Title="النتائج والمراجعة";page.Subtitle="الصفر درجة فعلية؛ عدم الدخول يظهر بحالته";
  if(from.HasValue&&to.HasValue&&from>to)ModelState.AddModelError("","تاريخ البداية يجب أن يسبق النهاية.");
  return View(page);
 }
}





