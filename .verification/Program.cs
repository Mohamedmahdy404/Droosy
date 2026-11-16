using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Droosy_DAL.Seeding;
using Droosy_PL.Models;
using Droosy_PL.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
if(args.Contains("--visitor-probe")){VisitorProbe.Run();FeatureChecks.Run();await UploadChecks.RunAsync();return;}
var publicationCases = new[]
{
    ("draft", PublicationStatus.Draft, (DateTime?)null, false),
    ("published", PublicationStatus.Published, (DateTime?)null, true),
    ("future scheduled", PublicationStatus.Scheduled, (DateTime?)DateTime.UtcNow.AddDays(1), false),
    ("due scheduled", PublicationStatus.Scheduled, (DateTime?)DateTime.UtcNow.AddMinutes(-1), true),
    ("scheduled without date", PublicationStatus.Scheduled, (DateTime?)null, false)
};
foreach (var (name, status, at, expected) in publicationCases)
{
    if (Droosy_BL.LearningService.IsPublished(new Lesson { PublicationStatus = status, PublishAtUtc = at }) != expected)
        throw new Exception("Publication rule failed: " + name);
}
Console.WriteLine("PUBLICATION_CHECKS_PASSED: draft, published, future scheduled, due scheduled, missing date");
TrackingChecks.Run();
VisitorProbe.Run();
FeatureChecks.Run();
await UploadChecks.RunAsync();
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddScoped<DemoScope>();
builder.Services.AddControllersWithViews().AddApplicationPart(typeof(DemoController).Assembly);
builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
builder.Services.Replace(ServiceDescriptor.Singleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider()));
var app=builder.Build();
using(var db=new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=localhost;Database=Droosy;Integrated Security=True;TrustServerCertificate=True").Options)) {
 if(db.Database.HasPendingModelChanges())throw new Exception("Model has unscaffolded changes");
 var script=db.GetService<IMigrator>().GenerateScript(options:MigrationsSqlGenerationOptions.Idempotent);
 if(!script.Contains("CREATE TABLE [Lessons]")||!script.Contains("InitialDemo"))throw new Exception("Migration script incomplete");
 if(db.Model.GetEntityTypes().Count()!=28)throw new Exception("Entity model count mismatch");
 if(DemoSeedData.Users.Length!=10||DemoSeedData.Subscriptions.Sum(x=>x.Price)!=2200||DemoSeedData.Subscriptions.Sum(x=>x.Paid)!=1450)throw new Exception("Seed totals mismatch");
 Console.WriteLine("CHECKS_OK: SQL migration generated without DB; 28 entities, 10 accounts, prices=2200 paid=1450 remaining=750");
}
app.UseStaticFiles(new StaticFileOptions{FileProvider=new Microsoft.Extensions.FileProviders.PhysicalFileProvider("D:/Projects/Droosy/Droosy/Droosy_PL/wwwroot")});
app.MapControllerRoute("preview","{controller=Preview}/{action=Index}/{id?}");

using(var scope=app.Services.CreateScope()) {
 var engine=scope.ServiceProvider.GetRequiredService<IRazorViewEngine>();
 var factory=new PreviewController();
 var checks=new List<(string Name,string View,object Model,string Expected)> {
 ("welcome","Welcome",factory.Fixture(),"أنا ولي أمر"),
 ("teacher home","Dashboard",factory.Fixture(),"أستاذ محمد"),
 ("parent Ahmed","Dashboard",factory.Fixture("Parent"),"أحمد"),
 ("parent Yassin","Dashboard",factory.Fixture("Parent","home","S02"),"ياسين"),
 ("student home","Dashboard",factory.Fixture("Student"),"أحمد"),
 ("teacher courses","Dashboard",factory.Fixture("Teacher","courses"),"أساسيات الجبر"),
 ("teacher groups","Dashboard",factory.Fixture("Teacher","groups"),"مجموعة الهندسة"),
 ("subscriptions","Dashboard",factory.Fixture("Teacher","subscriptions"),"الاشتراكات"),
 ("expired subscription","Dashboard",factory.Fixture("Student","subscriptions","S04"),"منتهٍ"),
 ("more","Dashboard",factory.Fixture("Parent","more"),"تبديل الدور")};
 var teacher=factory.Fixture();teacher.Courses=[teacher.Courses.First()];checks.Add(("teacher course","Course",teacher,"مسودة"));
 var student=factory.Fixture("Student");student.Courses=[student.Courses.First()];checks.Add(("student course","Course",student,"حل معادلة"));
 var group=factory.Fixture();group.Groups=[group.Groups.First()];checks.Add(("group members","Group",group,"عمر خالد"));
 var lesson=factory.Fixture("Student");lesson.Lesson=lesson.Courses.First().Units.First().Lessons.Last();lesson.CanOpen=true;checks.Add(("lesson preview","Lesson",lesson,"شرح مرئي"));
 var locked=factory.Fixture("Student","courses","S04");locked.Lesson=locked.Courses.First().Units.First().Lessons.Last();locked.CanOpen=false;checks.Add(("locked lesson","Lesson",locked,"اشتراكًا ساريًا"));
 checks.Add(("lesson editor","Edit",new EditLessonModel{Id="ALG-L04",CourseId="ALG",Title="مراجعة الجبر",Description="وصف الدرس",Status=PublicationStatus.Draft},"حفظ تعديلات"));
 var parentAttendance=factory.Fixture("Parent","attendance");checks.Add(("parent attendance","Attendance",parentAttendance,"غائب"));
 var teacherAttendance=factory.Fixture("Teacher","attendance");checks.Add(("teacher attendance","Attendance",teacherAttendance,"قادمة"));
 var detail=factory.Fixture("Parent","attendance");detail.SelectedSession=detail.Sessions.First();detail.Title="تفاصيل الحصة";checks.Add(("attendance detail","AttendanceDetail",detail,"تفاصيل"));
 checks.Add(("draft register","AttendanceEditor",TrackingChecks.Editor(factory.Fixture(),"A3"),"اعتماد الحضور"));
 checks.Add(("future register","AttendanceEditor",TrackingChecks.Editor(factory.Fixture(),"A4"),"الحصة لسه"));
 checks.Add(("parent progress","Progress",factory.Fixture("Parent","progress"),"بدأ التعلم"));
 checks.Add(("teacher progress","Progress",factory.Fixture("Teacher","progress"),"اختر الطالب"));
 var textLesson=factory.Fixture("Student");textLesson.Lesson=textLesson.Courses.First().Units.First().Lessons.First();textLesson.CanOpen=true;textLesson.CurrentProgress=textLesson.ProgressRows.First(x=>x.LessonId==textLesson.Lesson.Id);checks.Add(("completed text","Lesson",textLesson,"أكملت الدرس"));
 var parentLesson=factory.Fixture("Parent");parentLesson.Lesson=parentLesson.Courses.First().Units.First().Lessons.Last();parentLesson.CanOpen=true;checks.Add(("parent read only lesson","Lesson",parentLesson,"لا يغير تقدم الطالب"));
 var approved=TrackingChecks.Editor(factory.Fixture(),"A2");approved.History=[new AuditLog{Action="ApproveAttendance",CreatedAtUtc=DateTime.UtcNow,BeforeJson="[]",AfterJson=Droosy_BL.AttendanceService.Snapshot(approved.Session.Attendances,true)}];checks.Add(("approved register history","AttendanceEditor",approved,"سجل الاعتماد والتصحيح"));
 var historical=factory.Fixture("Student","courses","S07");historical.Lesson=historical.Courses.First().Units.Last().Lessons.First();historical.CanOpen=false;historical.CurrentProgress=historical.ProgressRows.Single(x=>x.LessonId==historical.Lesson.Id);checks.Add(("expired retained history","Lesson",historical,"تقدمك السابق محفوظ"));
 checks.AddRange(FeatureRenderCases.Create());
 foreach(var check in checks) {
  var http=new DefaultHttpContext { RequestServices=scope.ServiceProvider };http.Request.Scheme="http";http.Request.Host=new HostString("localhost");
  var route=new Microsoft.AspNetCore.Routing.RouteData();route.Routers.Add(new Microsoft.AspNetCore.Routing.RouteCollection());route.Values["controller"]="Demo";route.Values["action"]=check.View=="Welcome"?"Index":check.View;
  var action=new ActionContext(http,route,new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
  var found=engine.GetView(null,"/Views/Demo/"+check.View+".cshtml",true);if(!found.Success)throw new Exception("View not found "+check.View);
  var data=new ViewDataDictionary(new EmptyModelMetadataProvider(),new ModelStateDictionary()){Model=check.Model};if(check.View=="Edit" || check.View=="AttendanceEditor")data["Page"]=factory.Fixture();
  if(check.Model is AssignmentPageModel ap)data["Page"]=ap.Page;
  if(check.Model is ExamPageModel ep)data["Page"]=ep.Page;
  if(check.Model is ChatPageModel cp)data["Page"]=cp.Page;
  var temp=new TempDataDictionary(http,scope.ServiceProvider.GetRequiredService<ITempDataProvider>());
  using var output=new StringWriter();var context=new ViewContext(action,found.View,data,temp,output,new HtmlHelperOptions());await found.View.RenderAsync(context);
  var html=System.Net.WebUtility.HtmlDecode(output.ToString());if(!html.Contains(check.Expected))throw new Exception("Missing text "+check.Name+": "+check.Expected);
  if(!html.Contains("width=device-width")||!html.Contains("dir=\"rtl\""))throw new Exception("Missing mobile metadata "+check.Name);
  if(check.Name=="student course"&&html.Contains("مراجعة الجبر"))throw new Exception("Draft exposed");
  if(check.Name=="lesson editor"&&(!html.Contains("bottom-nav")||!html.Contains("data-show-hints")||!html.Contains("/Demo/Course/ALG")))throw new Exception("Editor navigation or help missing");
  if(check.Name=="parent attendance" && html.Contains("حصة الجبر · A3"))throw new Exception("Draft session leaked to parent");
  if(check.Name=="future register" && html.Contains("name=\"intent\""))throw new Exception("Future register editable");
  if(check.Name=="parent read only lesson" && (html.Contains("data-lesson-video") || html.Contains("/Demo/CompleteText")))throw new Exception("Parent can change progress");
  if(check.Name=="locked lesson" && html.Contains("/Demo/Material/"))throw new Exception("Locked resource exposed in HTML");
  if(check.Name=="unpublished exam"&&(html.Contains("answer-review")||html.Contains("published-grade")))throw new Exception("Unpublished exam exposed");
  if(check.Name=="ongoing timed exam"&&(!html.Contains("Z\" data-server-now")||html.Contains("answer-review")))throw new Exception("Exam deadline or answer visibility invalid");
  Console.WriteLine("RENDER_PASS: "+check.Name);
 }
}
Console.WriteLine("ALL_RENDER_CHECKS_PASSED");
if (args.Contains("--serve")) { app.Urls.Add("http://127.0.0.1:5258"); app.Run(); }

public class PreviewController:Controller {
 public DemoPage Fixture(string role="Teacher",string section="home",string student="S01")=>FinalFixture.Create(role,section,student);
 public IActionResult Index()=>View("/Views/Demo/Welcome.cshtml",Fixture());
 public IActionResult Screen(string role="Teacher",string section="home",string student="S01")=>View("/Views/Demo/Dashboard.cshtml",Fixture(role,section,student));
 public IActionResult Course(string role="Teacher",string student="S01"){var p=Fixture(role,"courses",student);p.Courses=[p.Courses.First()];return View("/Views/Demo/Course.cshtml",p);}
 public IActionResult Group(){var p=Fixture();p.Groups=[p.Groups.First()];return View("/Views/Demo/Group.cshtml",p);}
 public IActionResult Lesson(string role="Student",string student="S01",bool locked=false){var p=Fixture(role,"courses",student);p.Lesson=p.Courses.First().Units.First().Lessons.Last();p.CanOpen=!locked;return View("/Views/Demo/Lesson.cshtml",p);}
 public IActionResult Edit(){ViewData["Page"]=Fixture();ViewData["Title"]="تعديل الدرس";return View("/Views/Demo/Edit.cshtml",new EditLessonModel{Id="ALG-L04",CourseId="ALG",Title="مراجعة الجبر",Description="مراجعة شاملة على المعادلات مع أمثلة وتطبيقات متنوعة.",Status=PublicationStatus.Draft});}
}






