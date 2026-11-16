using Droosy_BL;
using Droosy_DAL.Entities;
using Droosy_DAL.Seeding;
using Droosy_PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Droosy_PL.Controllers;

public partial class DemoController
{
    public async Task<IActionResult> Attendance(DateOnly? from, DateOnly? to)
    {
        var page = await Page("attendance");
        if (page.Account == null) return RedirectToAction(nameof(Index));
        page.Title = page.IsTeacher ? "الحصص والحضور" : "الحضور والغياب";
        page.Subtitle = page.IsTeacher ? "سجّل الحضور، ثم اعتمده ليظهر للأسرة" : "متابعة واضحة للحصص المعتمدة";
        page.From = from;
        page.To = to;
        if (from.HasValue && to.HasValue && from > to) ModelState.AddModelError("", "تاريخ البداية يجب أن يسبق تاريخ النهاية.");
        if (ModelState.IsValid)
        {
            if (from.HasValue) page.Sessions = page.Sessions.Where(x => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(x.StartsAtUtc, DateTimeKind.Utc), Cairo)) >= from).ToList();
            if (to.HasValue) page.Sessions = page.Sessions.Where(x => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(x.StartsAtUtc, DateTimeKind.Utc), Cairo)) <= to).ToList();
        }
        return View(page);
    }

    public async Task<IActionResult> Session(string id)
    {
        var page = await Page("attendance");
        if (page.Account == null) return RedirectToAction(nameof(Index));
        var session = page.Sessions.SingleOrDefault(x => x.Id == id);
        if (session == null) return NotFound();
        if (!page.IsTeacher)
        {
            page.SelectedSession = session;
            page.Title = "تفاصيل الحصة";
            page.Subtitle = session.Group.Name;
            return View("AttendanceDetail", page);
        }
        return await AttendanceEditor(page, session);
    }

    private async Task<IActionResult> AttendanceEditor(DemoPage page, ClassSession session, AttendanceEditorModel? posted = null)
    {
        var members = page.Groups.Single(x => x.Id == session.GroupId).Students.OrderBy(x => x.StudentId).ToList();
        var model = posted ?? new AttendanceEditorModel { SessionId = session.Id, Entries = members.Select(x =>
        {
            var row = session.Attendances.SingleOrDefault(a => a.StudentId == x.StudentId);
            return new AttendanceInput { StudentId = x.StudentId, StudentName = x.Student.DisplayName, Status = row?.Status, Note = row?.Note ?? "" };
        }).ToList() };
        model.Session = session;
        foreach (var entry in model.Entries) entry.StudentName = members.FirstOrDefault(x => x.StudentId == entry.StudentId)?.Student.DisplayName ?? "طالب غير موجود";
        model.History = await db.AuditLogs.AsNoTracking().Where(x => x.EntityType == nameof(ClassSession) && x.EntityId == session.Id).OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
        ViewData["Page"] = page;
        ViewData["Title"] = "تسجيل الحضور";
        return View("AttendanceEditor", model);
    }

    [HttpPost]
    public async Task<IActionResult> SaveAttendance(AttendanceEditorModel model, string intent)
    {
        var page = await Page("attendance");
        if (!page.IsTeacher) return BadRequest("اختر حساب المدرس أولًا.");
        var session = page.Sessions.SingleOrDefault(x => x.Id == model.SessionId);
        if (session == null) return NotFound();
        if (intent is not ("draft" or "approve")) return BadRequest();
        if (model.Entries.Any(x => !x.Status.HasValue)) ModelState.AddModelError("", "اختر حالة الحضور لكل طالب.");
        if (ModelState.IsValid)
        {
            var error = await attendance.SaveAsync(model.SessionId, page.Account!.Id,
                model.Entries.Select(x => new AttendanceEntry(x.StudentId, x.Status!.Value, x.Note ?? "")).ToList(), intent == "approve");
            if (error == null)
            {
                TempData["Success"] = intent == "approve" ? "تم اعتماد الحضور. يمكنك متابعة النتيجة من حساب ولي الأمر." : "تم حفظ المسودة. النسخة المعتمدة عند الأهل لم تتغير.";
                return RedirectToAction(nameof(Session), new { id = model.SessionId });
            }
            ModelState.AddModelError("", error);
        }
        return await AttendanceEditor(page, session, model);
    }

    public async Task<IActionResult> Progress(string? studentId)
    {
        var page = await Page("progress");
        if (page.Account == null) return RedirectToAction(nameof(Index));
        if (page.IsTeacher)
        {
            page.Student = page.Students.SingleOrDefault(x => x.Id == studentId) ?? page.Students.First();
            page.Courses = await learning.CoursesForAsync(page.Student.Id);
        }
        page.Title = page.IsParent ? "متابعة الدروس" : page.IsTeacher ? "تقدم الطلاب" : "رحلتي في التعلم";
        page.Subtitle = "حالة التعلم المسجلة؛ المشاهدة لا تعني قياس الفهم";
        return View(page);
    }

    private async Task<(DemoPage Page, Lesson? Lesson)> AccessibleLesson(string id)
    {
        var page = await Page("courses");
        var lesson = page.Courses.SelectMany(x => x.Units).SelectMany(x => x.Lessons).SingleOrDefault(x => x.Id == id);
        if (lesson == null || (!page.IsTeacher && (!LearningService.IsPublished(lesson) || !await learning.CanOpenAsync(page.Student!.Id, lesson))))
            return (page, null);
        return (page, lesson);
    }

    public async Task<IActionResult> Material(string id, string kind = "pdf", bool download = false)
    {
        var (page, lesson) = await AccessibleLesson(id);
        if (page.Account == null) return RedirectToAction(nameof(Index));
        if (lesson == null) return NotFound();
        if (kind is not ("pdf" or "poster") && (kind != "video" || lesson.ContentType != LessonContentType.Video)) return NotFound();
        var extension = kind == "pdf" ? ".pdf" : kind == "poster" ? ".png" : ".mp4";
        var path = Path.Combine(environment.ContentRootPath, "App_Data", "Lessons", lesson.Id + extension);
        if (!System.IO.File.Exists(path)) return NotFound();
        if (download && kind == "pdf") return PhysicalFile(path, "application/pdf", lesson.Title + extension);
        return PhysicalFile(path, kind == "pdf" ? "application/pdf" : kind == "poster" ? "image/png" : "video/mp4", enableRangeProcessing: true);
    }

    [HttpPost]
    public async Task<IActionResult> CompleteText(string id)
    {
        var (page, lesson) = await AccessibleLesson(id);
        if (lesson == null || page.IsParent || page.IsTeacher || lesson.ContentType != LessonContentType.Text) return BadRequest();
        await progress.OpenAsync(page.Student!.Id, id, complete: true);
        TempData["Success"] = "خطوة جميلة! تم حفظ إكمال الدرس، وسيظهر عند ولي الأمر.";
        return RedirectToAction(nameof(Lesson), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> SaveVideoProgress(string id, [FromBody] VideoProgressInput input)
    {
        var (page, lesson) = await AccessibleLesson(id);
        if (lesson == null || page.IsParent || page.IsTeacher || lesson.ContentType != LessonContentType.Video) return BadRequest();
        if (input == null || !double.IsFinite(input.Position) || input.Played == null || input.Played.Count > 300 || input.Played.Any(x => x == null || x.Length != 2 || x.Any(n => !double.IsFinite(n)))) return BadRequest();
        var row = await progress.SaveVideoAsync(page.Student!.Id, id, input.Position, input.Played);
        return Json(new { completed = row.IsCompleted, watchedSeconds = ProgressService.WatchedSeconds(row), position = row.LastPositionSeconds });
    }
}
