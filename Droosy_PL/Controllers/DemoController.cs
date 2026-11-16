using Droosy_BL;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Droosy_PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Droosy_PL.Controllers;

public class DemoController(ApplicationDbContext db, LearningService learning) : Controller
{
    private static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

    private async Task<DemoPage> Page(string section = "home")
    {
        var accountId = Request.Cookies["droosy-account"];
        var page = new DemoPage
        {
            Account = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == accountId),
            Section = section
        };
        if (page.Account == null) return page;

        page.Students = page.IsParent
            ? await db.ParentStudents.AsNoTracking().Where(x => x.ParentId == page.Account.Id)
                .Select(x => x.Student).OrderBy(x => x.Id).ToListAsync()
            : page.IsTeacher
                ? await db.Users.AsNoTracking().Where(x => x.Role == DemoRole.Student).OrderBy(x => x.Id).ToListAsync()
                : [page.Account];
        page.Student = page.Students.FirstOrDefault(x => x.Id == Request.Cookies["droosy-child"])
            ?? page.Students.FirstOrDefault();
        page.Courses = page.IsTeacher ? await learning.Courses.OrderBy(x => x.Id).ToListAsync()
            : page.Student == null ? [] : await learning.CoursesForAsync(page.Student.Id);
        var studentId = page.Student?.Id;
        page.Groups = await db.StudyGroups.AsNoTracking()
            .Include(x => x.Course).ThenInclude(x => x.Level)
            .Include(x => x.Students).ThenInclude(x => x.Student)
            .Where(x => page.IsTeacher || x.Students.Any(s => s.StudentId == studentId))
            .OrderBy(x => x.Id).ToListAsync();
        page.Subscriptions = await db.Subscriptions.AsNoTracking()
            .Include(x => x.Student).Include(x => x.Course).Include(x => x.Payments)
            .Where(x => page.IsTeacher || x.StudentId == studentId)
            .OrderBy(x => x.StudentId).ToListAsync();
        return page;
    }

    public async Task<IActionResult> Index() => View("Welcome", await Page());

    [HttpPost]
    public async Task<IActionResult> Enter(string accountId)
    {
        if (!await db.Users.AnyAsync(x => x.Id == accountId)) return BadRequest();
        Response.Cookies.Append("droosy-account", accountId,
            new CookieOptions { Path = "/", IsEssential = true, SameSite = SameSiteMode.Lax });
        Response.Cookies.Delete("droosy-child");
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    public async Task<IActionResult> Child(string studentId, string section = "home")
    {
        var page = await Page();
        if (page.Account == null) return RedirectToAction(nameof(Index));
        if (!page.Students.Any(x => x.Id == studentId)) return BadRequest();
        Response.Cookies.Append("droosy-child", studentId,
            new CookieOptions { Path = "/", IsEssential = true, SameSite = SameSiteMode.Lax });
        // Keep the current tab while changing children, including from a course detail.
        if (!new[] { "home", "courses", "groups", "subscriptions", "more" }.Contains(section)) section = "home";
        return RedirectToAction(nameof(Dashboard), new { section });
    }

    public async Task<IActionResult> Dashboard(string section = "home")
    {
        if (!new[] { "home", "courses", "groups", "subscriptions", "more" }.Contains(section)) return NotFound();
        var page = await Page(section);
        if (page.Account == null) return RedirectToAction(nameof(Index));
        page.Title = section switch
        {
            "courses" => page.IsTeacher ? "المحتوى التعليمي" : page.IsParent ? "دروس ابني" : "كورساتي",
            "groups" => "المجموعات",
            "subscriptions" => "الاشتراكات",
            "more" => "المزيد",
            _ => page.IsTeacher ? "أهلًا، أستاذ محمد" : page.IsParent ? "متابعة أبنائي" : "أهلًا، " + page.Student?.DisplayName.Split(' ')[0]
        };
        page.Subtitle = page.IsParent ? "قريب من تعلّمه، في كل خطوة" : "كل خطوة صغيرة تصنع فرقًا";
        if (page.IsTeacher && section == "subscriptions")
            page.Codes = await db.ActivationCodes.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        return View(page);
    }

    public async Task<IActionResult> Course(string id)
    {
        var page = await Page("courses");
        if (page.Account == null) return RedirectToAction(nameof(Index));
        var course = page.Courses.SingleOrDefault(x => x.Id == id);
        if (course == null) return NotFound();
        page.Courses = [course];
        page.Title = course.Title;
        page.Subtitle = course.Level.Name;
        return View(page);
    }

    public async Task<IActionResult> Group(string id)
    {
        var page = await Page("groups");
        if (page.Account == null) return RedirectToAction(nameof(Index));
        var group = page.Groups.SingleOrDefault(x => x.Id == id);
        if (group == null) return NotFound();
        page.Groups = [group];
        page.Title = group.Name;
        page.Subtitle = group.Schedule;
        return View(page);
    }

    public async Task<IActionResult> Lesson(string id)
    {
        var page = await Page("courses");
        if (page.Account == null) return RedirectToAction(nameof(Index));
        page.Lesson = page.Courses.SelectMany(x => x.Units).SelectMany(x => x.Lessons).SingleOrDefault(x => x.Id == id);
        if (page.Lesson == null || (!page.IsTeacher && !LearningService.IsPublished(page.Lesson))) return NotFound();
        page.CanOpen = page.IsTeacher || await learning.CanOpenAsync(page.Student!.Id, page.Lesson);
        page.Title = page.CanOpen ? page.Lesson.Title : "المحتوى مقيد";
        page.Subtitle = page.Lesson.Unit.Course.Title;
        return View(page);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var page = await Page("courses");
        if (!page.IsTeacher) return RedirectToAction(nameof(Index));
        var lesson = await db.Lessons.AsNoTracking().Include(x => x.Unit).SingleOrDefaultAsync(x => x.Id == id);
        if (lesson == null) return NotFound();
        ViewData["Page"] = page;
        ViewData["Title"] = "تعديل الدرس";
        return View(new EditLessonModel
        {
            Id = id, CourseId = lesson.Unit.CourseId, Title = lesson.Title, Description = lesson.Description,
            Status = lesson.PublicationStatus, IsPreview = lesson.IsPreview,
            PublishAtLocal = lesson.PublishAtUtc.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(lesson.PublishAtUtc.Value, DateTimeKind.Utc), Cairo) : null
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditLessonModel model)
    {
        var page = await Page("courses");
        if (!page.IsTeacher) return BadRequest("اختر حساب المدرس أولًا");
        var lesson = await db.Lessons.AsNoTracking().Include(x => x.Unit).SingleOrDefaultAsync(x => x.Id == model.Id);
        if (lesson == null) return NotFound();
        model.CourseId = lesson.Unit.CourseId;
        if (!Enum.IsDefined(model.Status)) ModelState.AddModelError(nameof(model.Status), "اختر حالة نشر صحيحة");
        if ((model.Title?.Trim().Length ?? 0) < 3)
            ModelState.AddModelError(nameof(model.Title), "اكتب عنوانًا من ٣ أحرف على الأقل");

        DateTime? publishAtUtc = null;
        if (model.Status == PublicationStatus.Scheduled)
        {
            if (model.PublishAtLocal.HasValue)
            {
                var local = DateTime.SpecifyKind(model.PublishAtLocal.Value, DateTimeKind.Unspecified);
                if (Cairo.IsInvalidTime(local) || Cairo.IsAmbiguousTime(local))
                    ModelState.AddModelError(nameof(model.PublishAtLocal), "اختر ساعة أخرى؛ هذا الموعد يتزامن مع تغيير التوقيت الصيفي");
                else publishAtUtc = TimeZoneInfo.ConvertTimeToUtc(local, Cairo);
            }
            if (!publishAtUtc.HasValue || publishAtUtc <= DateTime.UtcNow)
                ModelState.AddModelError(nameof(model.PublishAtLocal), "حدد موعدًا قادمًا بتوقيت القاهرة");
        }
        if (!ModelState.IsValid)
        {
            ViewData["Page"] = page;
            ViewData["Title"] = "تعديل الدرس";
            return View(model);
        }

        var courseId = await learning.EditAsync(model.Id, model.Title!, model.Description,
            model.Status, model.IsPreview, publishAtUtc);
        if (courseId == null) return NotFound();
        TempData["Success"] = model.Status == PublicationStatus.Published
            ? "تم حفظ الدرس ونشره. جرّب مشاهدته من حساب الطالب." : "تم حفظ تعديلات الدرس.";
        return RedirectToAction(nameof(Course), new { id = courseId });
    }
}

