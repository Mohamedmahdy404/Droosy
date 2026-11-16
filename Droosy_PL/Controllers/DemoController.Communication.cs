using Droosy_BL;
using Droosy_DAL.Entities;
using Droosy_PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Droosy_PL.Controllers;

public partial class DemoController
{
 public async Task<IActionResult> Messages(bool unread=false)
 {
  var page=await Page("messages");if(page.Account==null)return RedirectToAction(nameof(Index));if(unread)page.Conversations=page.Conversations.Where(x=>x.Messages.Any(m=>m.SenderId!=page.Account.Id&&m.ReadAtUtc==null)).ToList();page.Title="الرسائل";page.Subtitle="تواصل واضح، وسياق يجمع كل التفاصيل";return View(page);
 }
 private async Task<Conversation?> ConversationFor(DemoPage page,string id)=>page.Account==null?null:await db.Conversations.Include(x=>x.Student).Include(x=>x.OtherUser).Include(x=>x.Messages).ThenInclude(x=>x.Sender).SingleOrDefaultAsync(x=>x.Id==id&&(x.TeacherId==page.Account.Id||x.OtherUserId==page.Account.Id));
 public async Task<IActionResult> Chat(string id)
 {
  var page=await Page("messages");if(page.Account==null)return RedirectToAction(nameof(Index));var conversation=await ConversationFor(page,id);if(conversation==null)return NotFound();
  if(page.IsParent&&page.Student!.Id!=conversation.StudentId)
  {
   if(!page.Students.Any(x=>x.Id==conversation.StudentId))return NotFound();
   Response.Cookies.Append("droosy-child",conversation.StudentId,new CookieOptions{Path="/",IsEssential=true,SameSite=SameSiteMode.Lax});return RedirectToAction(nameof(Chat),new{id});
  }
  foreach(var message in conversation.Messages.Where(x=>x.SenderId!=page.Account.Id&&x.ReadAtUtc==null))message.ReadAtUtc=DateTime.UtcNow;
  await db.SaveChangesAsync();
  var title=conversation.ContextType switch {
   ConversationContext.Lesson=>await db.Lessons.Where(x=>x.Id==conversation.ContextId).Select(x=>x.Title).SingleAsync(),
   ConversationContext.Assignment=>await db.Assignments.Where(x=>x.Id==conversation.ContextId).Select(x=>x.Title).SingleAsync(),
   _=>await db.Exams.Where(x=>x.Id==conversation.ContextId).Select(x=>x.Title).SingleAsync()};
  ViewData["Page"]=page;ViewData["Title"]=page.IsTeacher?conversation.OtherUser.DisplayName:"أ. محمد عادل";
  return View(new ChatPageModel{Page=page,Conversation=conversation,ContextTitle=title});
 }
 [HttpPost]
 public async Task<IActionResult> BeginChat(ConversationContext contextType,string contextId,string? studentId,string? otherUserId)
 {
  var page=await Page("messages");if(page.Account==null)return RedirectToAction(nameof(Index));if(!Enum.IsDefined(contextType))return BadRequest();
  var student=page.IsTeacher?page.Students.SingleOrDefault(x=>x.Id==studentId):page.Student;if(student==null)return BadRequest();
  var course=contextType switch {
   ConversationContext.Lesson=>await db.Lessons.Where(x=>x.Id==contextId).Select(x=>x.Unit.CourseId).SingleOrDefaultAsync(),
   ConversationContext.Assignment=>await db.Assignments.Where(x=>x.Id==contextId).Select(x=>x.CourseId).SingleOrDefaultAsync(),
   _=>await db.Exams.Where(x=>x.Id==contextId).Select(x=>x.CourseId).SingleOrDefaultAsync()};
  if(course==null||!await db.GroupStudents.AnyAsync(x=>x.StudentId==student.Id&&x.Group.CourseId==course))return NotFound();
  if(contextType==ConversationContext.Exam&&!page.IsTeacher&&!await db.ExamAttempts.AnyAsync(x=>x.ExamId==contextId&&x.StudentId==student.Id&&x.PublishedAtUtc!=null))return NotFound();
  var other=page.IsTeacher?otherUserId??student.Id:page.Account.Id;
  if(page.IsTeacher&&!await db.Users.AnyAsync(x=>x.Id==other&&(x.Id==student.Id||db.ParentStudents.Any(p=>p.ParentId==other&&p.StudentId==student.Id))))return BadRequest();
  var conversation=await db.Conversations.SingleOrDefaultAsync(x=>x.TeacherId=="T01"&&x.OtherUserId==other&&x.StudentId==student.Id&&x.ContextType==contextType&&x.ContextId==contextId);
  if(conversation==null){conversation=new Conversation{Id=Guid.NewGuid().ToString("N"),OtherUserId=other,StudentId=student.Id,ContextType=contextType,ContextId=contextId};db.Conversations.Add(conversation);await db.SaveChangesAsync();}
  return RedirectToAction(nameof(Chat),new{id=conversation.Id});
 }
 [HttpPost,RequestSizeLimit(6*1024*1024)]
 public async Task<IActionResult> SendMessage(string id,Guid clientMessageId,string? text,IFormFile? attachment)
 {
  var page=await Page();var conversation=await ConversationFor(page,id);if(conversation==null)return NotFound();
  var duplicate=await db.Messages.SingleOrDefaultAsync(x=>x.Id==clientMessageId.ToString("N"));
  if(duplicate!=null)return duplicate.SenderId==page.Account!.Id&&duplicate.ConversationId==id?RedirectToAction(nameof(Chat),new{id}):BadRequest();
  if(clientMessageId==Guid.Empty)return BadRequest();
  if((text?.Length??0)>2000){TempData["Error"]="الرسالة لا تتجاوز ٢٠٠٠ حرف.";return RedirectToAction(nameof(Chat),new{id});}
  Droosy_PL.Services.SavedUpload? file;
  try{file=await uploads.SaveAsync(attachment);}catch(ArgumentException e){TempData["Error"]=e.Message;return RedirectToAction(nameof(Chat),new{id});}
  if(string.IsNullOrWhiteSpace(text)&&file==null){TempData["Error"]="اكتب رسالة أو اختر مرفقًا.";return RedirectToAction(nameof(Chat),new{id});}
  var message=new Message{Id=clientMessageId.ToString("N"),ConversationId=id,SenderId=page.Account!.Id,Text=text?.Trim()??"",SentAtUtc=DateTime.UtcNow,AttachmentPath=file?.Path,AttachmentName=file?.Name,AttachmentType=file?.ContentType};
  db.Messages.Add(message);var recipient=page.Account.Id==conversation.TeacherId?conversation.OtherUserId:conversation.TeacherId;
  await notifications.AddAsync(recipient,conversation.StudentId,"رسالة جديدة من "+page.Account.DisplayName,"/Demo/Chat/"+id,"Message:"+message.Id);
  try{await db.SaveChangesAsync();}catch{if(file!=null)uploads.Delete(file.Path);throw;}
  return RedirectToAction(nameof(Chat),new{id});
 }
 public async Task<IActionResult> MessageAttachment(string id)
 {
  var page=await Page();if(page.Account==null)return RedirectToAction(nameof(Index));
  var message=await db.Messages.Include(x=>x.Conversation).SingleOrDefaultAsync(x=>x.Id==id&&(x.Conversation.TeacherId==page.Account.Id||x.Conversation.OtherUserId==page.Account.Id));
  if(message?.AttachmentPath==null)return NotFound();var path=uploads.Resolve(message.AttachmentPath);if(!System.IO.File.Exists(path))return NotFound();
  return PhysicalFile(path,message.AttachmentType??"application/octet-stream",message.AttachmentName,enableRangeProcessing:true);
 }
 public async Task<IActionResult> Announcements()
 {
  var page=await Page("announcements");if(page.Account==null)return RedirectToAction(nameof(Index));page.Title="الإعلانات";page.Subtitle="كل جديد يخص رحلتك التعليمية";return View(page);
 }
 [HttpPost]
 public async Task<IActionResult> PublishAnnouncement(Guid clientId,string? groupId,string title,string body)
 {
  var page=await Page();if(!page.IsTeacher)return BadRequest();
  if(clientId==Guid.Empty)return BadRequest();if(await db.Announcements.AnyAsync(x=>x.Id==clientId.ToString("N")))return RedirectToAction(nameof(Announcements));
  if(string.IsNullOrWhiteSpace(title)||string.IsNullOrWhiteSpace(body)||title.Length>160||body.Length>2000){TempData["Error"]="اكتب عنوانًا حتى ١٦٠ حرفًا ونصًا حتى ٢٠٠٠ حرف.";return RedirectToAction(nameof(Announcements));}
  if(!string.IsNullOrEmpty(groupId)&&!page.Groups.Any(x=>x.Id==groupId))return BadRequest();
  var announcement=new Announcement{Id=clientId.ToString("N"),GroupId=string.IsNullOrEmpty(groupId)?null:groupId,Title=title.Trim(),Body=body.Trim(),PublishedAtUtc=DateTime.UtcNow,TargetUrl="/Demo/Announcements"};db.Announcements.Add(announcement);
  var students=groupId==null||groupId==""?page.Students.Select(x=>x.Id).ToList():page.Groups.Single(x=>x.Id==groupId).Students.Select(x=>x.StudentId).ToList();
  var parents=await db.ParentStudents.Where(x=>students.Contains(x.StudentId)).Select(x=>x.ParentId).Distinct().ToListAsync();
  foreach(var student in students)await notifications.AddAsync(student,student,"إعلان جديد: "+announcement.Title,"/Demo/Announcements","Announcement:"+announcement.Id);
  if(announcement.GroupId==null){foreach(var parent in parents)await notifications.AddAsync(parent,null,"إعلان جديد: "+announcement.Title,"/Demo/Announcements","Announcement:"+announcement.Id);}
  else foreach(var link in await db.ParentStudents.Where(x=>students.Contains(x.StudentId)).ToListAsync())await notifications.AddAsync(link.ParentId,link.StudentId,"إعلان جديد: "+announcement.Title,"/Demo/Announcements","Announcement:"+announcement.Id+":"+link.StudentId);
  await db.SaveChangesAsync();TempData["Success"]="تم نشر الإعلان للفئة المختارة.";return RedirectToAction(nameof(Announcements));
 }
 public async Task<IActionResult> Notifications()
 {
  var page=await Page("notifications");if(page.Account==null)return RedirectToAction(nameof(Index));page.Title="الإشعارات";page.Subtitle="آخر التحديثات التي تخصك";return View(page);
 }
 [HttpPost]
 public async Task<IActionResult> OpenNotification(string id)
 {
  var page=await Page();if(page.Account==null)return BadRequest();var row=await db.Notifications.SingleOrDefaultAsync(x=>x.Id==id&&x.UserId==page.Account.Id);if(row==null)return NotFound();
  if(page.IsParent&&row.StudentId!=null)
  {
   if(!page.Students.Any(x=>x.Id==row.StudentId))return NotFound();
   Response.Cookies.Append("droosy-child",row.StudentId,new CookieOptions{Path="/",IsEssential=true,SameSite=SameSiteMode.Lax});
  }
  row.ReadAtUtc??=DateTime.UtcNow;await db.SaveChangesAsync();return LocalRedirect(row.TargetUrl);
 }
 [HttpPost]
 public async Task<IActionResult> ReadNotifications()
 {
  var page=await Page();if(page.Account==null)return BadRequest();var ids=page.Notifications.Select(x=>x.Id).ToArray();
  foreach(var row in await db.Notifications.Where(x=>ids.Contains(x.Id)&&x.ReadAtUtc==null).ToListAsync())row.ReadAtUtc=DateTime.UtcNow;
  await db.SaveChangesAsync();return RedirectToAction(nameof(Notifications));
 }
 public async Task<IActionResult> Weekly(DateOnly? until)
 {
  var page=await Page("weekly");if(page.Account==null)return RedirectToAction(nameof(Index));
  page.WeekToUtc=until.HasValue?TimeZoneInfo.ConvertTimeToUtc(until.Value.AddDays(1).ToDateTime(TimeOnly.MinValue),Cairo).AddTicks(-1):DateTime.UtcNow;
  if(page.WeekToUtc>DateTime.UtcNow)page.WeekToUtc=DateTime.UtcNow;
  page.WeekFromUtc=page.WeekToUtc.AddDays(-7);page.To=until;page.Title="التقرير الأسبوعي";page.Subtitle="أسبوع من الخطوات الصغيرة، في صورة واحدة";return View(page);
 }
 public async Task<IActionResult> Students(string? id)
 {
  var page=await Page("students");if(!page.IsTeacher)return RedirectToAction(nameof(Index));
  if(id!=null){page.Student=page.Students.SingleOrDefault(x=>x.Id==id);if(page.Student==null)return NotFound();page.Courses=await learning.CoursesForAsync(id);page.Submissions=page.Submissions.Where(x=>x.StudentId==id).ToList();page.Attempts=page.Attempts.Where(x=>x.StudentId==id).ToList();page.ProgressRows=page.ProgressRows.Where(x=>x.StudentId==id).ToList();page.Title=page.Student.DisplayName;}
  else page.Title="طلابك";
  page.Subtitle="التعلم والحضور والنتائج في ملف واحد";ViewData["StudentDetail"]=id!=null;return View(page);
 }
 public async Task<IActionResult> Review()
 {
  var page=await Page("review");if(!page.IsTeacher)return RedirectToAction(nameof(Index));page.Title="بانتظار مراجعتك";page.Subtitle="التسليمات والنتائج التي تحتاج خطوتك التالية";return View(page);
 }
 public static List<AttentionItem> BuildAttention(DemoPage page)
 {
  var items=new List<AttentionItem>();var now=DateTime.UtcNow;if(page.IsTeacher||page.Student==null)return items;
  var last=page.Sessions.Where(x=>x.IsApproved).OrderByDescending(x=>x.StartsAtUtc).FirstOrDefault();var attendance=last?.Attendances.SingleOrDefault(x=>x.StudentId==page.Student.Id);
  if(attendance?.PublishedStatus==AttendanceStatus.Absent)items.Add(new("آخر حضور يحتاج متابعة",last!.Title,"/Demo/Session/"+last.Id,"danger"));
  foreach(var work in page.Assignments.Where(x=>x.DueAtUtc<now))
  {
   var sub=page.Submissions.SingleOrDefault(x=>x.AssignmentId==work.Id&&x.StudentId==page.Student.Id);
   if(sub==null||sub.Status is SubmissionStatus.Draft or SubmissionStatus.Returned)items.Add(new(sub?.Status==SubmissionStatus.Returned?"واجب معاد للتعديل":"واجب متأخر",work.Title,"/Demo/Assignment/"+work.Id,"amber"));
  }
  foreach(var exam in page.Exams.Where(x=>x.ClosesAtUtc<now&&!page.Attempts.Any(a=>a.ExamId==x.Id&&a.StudentId==page.Student.Id)))items.Add(new("اختبار لم يدخل إليه الطالب",exam.Title,"/Demo/Exam/"+exam.Id,"amber"));
  foreach(var subscription in page.Subscriptions.Where(x=>x.StudentId==page.Student.Id&&x.EndsAtUtc>now&&x.EndsAtUtc<=now.AddDays(7)))items.Add(new("الاشتراك ينتهي قريبًا",DemoPage.Date(subscription.EndsAtUtc),"/Demo/Dashboard?section=subscriptions","amber"));
  return items;
 }
}


