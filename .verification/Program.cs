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
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews().AddApplicationPart(typeof(DemoController).Assembly);
builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
builder.Services.Replace(ServiceDescriptor.Singleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider()));
var app=builder.Build();
using(var db=new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=localhost;Database=Droosy;Integrated Security=True;TrustServerCertificate=True").Options)) {
 if(db.Database.HasPendingModelChanges())throw new Exception("Model has unscaffolded changes");
 var script=db.GetService<IMigrator>().GenerateScript(options:MigrationsSqlGenerationOptions.Idempotent);
 if(!script.Contains("CREATE TABLE [Lessons]")||!script.Contains("InitialDemo"))throw new Exception("Migration script incomplete");
 if(db.Model.GetEntityTypes().Count()!=13)throw new Exception("Entity model count mismatch");
 if(DemoSeedData.Users.Length!=10||DemoSeedData.Subscriptions.Sum(x=>x.Price)!=2200||DemoSeedData.Subscriptions.Sum(x=>x.Paid)!=1450)throw new Exception("Seed totals mismatch");
 Console.WriteLine("CHECKS_OK: SQL migration generated without DB; 13 entities, 10 accounts, prices=2200 paid=1450 remaining=750");
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
 var lesson=factory.Fixture("Student");lesson.Lesson=lesson.Courses.First().Units.First().Lessons.Last();lesson.CanOpen=true;checks.Add(("lesson preview","Lesson",lesson,"معاينة"));
 var locked=factory.Fixture("Student","courses","S04");locked.Lesson=locked.Courses.First().Units.First().Lessons.Last();locked.CanOpen=false;checks.Add(("locked lesson","Lesson",locked,"اشتراكًا ساريًا"));
 checks.Add(("lesson editor","Edit",new EditLessonModel{Id="ALG-L04",CourseId="ALG",Title="مراجعة الجبر",Description="وصف الدرس",Status=PublicationStatus.Draft},"حفظ تعديلات"));
 foreach(var check in checks) {
  var http=new DefaultHttpContext { RequestServices=scope.ServiceProvider };http.Request.Scheme="http";http.Request.Host=new HostString("localhost");
  var route=new Microsoft.AspNetCore.Routing.RouteData();route.Routers.Add(new Microsoft.AspNetCore.Routing.RouteCollection());route.Values["controller"]="Demo";route.Values["action"]=check.View=="Welcome"?"Index":check.View;
  var action=new ActionContext(http,route,new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
  var found=engine.GetView(null,"/Views/Demo/"+check.View+".cshtml",true);if(!found.Success)throw new Exception("View not found "+check.View);
  var data=new ViewDataDictionary(new EmptyModelMetadataProvider(),new ModelStateDictionary()){Model=check.Model};if(check.View=="Edit")data["Page"]=factory.Fixture();
  var temp=new TempDataDictionary(http,scope.ServiceProvider.GetRequiredService<ITempDataProvider>());
  using var output=new StringWriter();var context=new ViewContext(action,found.View,data,temp,output,new HtmlHelperOptions());await found.View.RenderAsync(context);
  var html=System.Net.WebUtility.HtmlDecode(output.ToString());if(!html.Contains(check.Expected))throw new Exception("Missing text "+check.Name+": "+check.Expected);
  if(!html.Contains("width=device-width")||!html.Contains("dir=\"rtl\""))throw new Exception("Missing mobile metadata "+check.Name);
  if(check.Name=="student course"&&html.Contains("مراجعة الجبر"))throw new Exception("Draft exposed");
  if(check.Name=="lesson editor"&&(!html.Contains("bottom-nav")||!html.Contains("data-show-hints")||!html.Contains("/Demo/Course/ALG")))throw new Exception("Editor navigation or help missing");
  Console.WriteLine("RENDER_PASS: "+check.Name);
 }
}
Console.WriteLine("ALL_RENDER_CHECKS_PASSED");
if (args.Contains("--serve")) { app.Urls.Add("http://127.0.0.1:5258"); app.Run(); }

public class PreviewController:Controller {
 public DemoPage Fixture(string role="Teacher",string section="home",string student="S01") {
 var users=DemoSeedData.Users.ToDictionary(x=>x.Id,x=>new AppUser{Id=x.Id,DisplayName=x.Name,Role=x.Role});
 var courses=DemoSeedData.Content.Select(x=>new Course{Id=x.Course,Title=x.Course=="ALG"?"أساسيات الجبر":"أساسيات الهندسة",Description="دروس مرتبة تساعدك على التعلم خطوة بخطوة.",MonthlyPrice=x.Course=="ALG"?300:250,Level=new AcademicLevel{Name=x.Course=="ALG"?"الصف الثاني الإعدادي":"الصف الأول الإعدادي"},Units=x.Units.Select((title,i)=>new CourseUnit{Id=x.Course+"-U"+(i+1),CourseId=x.Course,Title=title,SortOrder=i+1}).ToList()}).ToList();
 foreach(var c in courses){var content=DemoSeedData.Content.Single(x=>x.Course==c.Id);foreach(var unit in c.Units)unit.Course=c;for(int i=0;i<4;i++){var unit=c.Units[i/2];unit.Lessons.Add(new Lesson{Id=c.Id+"-L0"+(i+1),UnitId=unit.Id,Unit=unit,Title=content.Lessons[i],Description=content.Descriptions[i],SortOrder=i+1,IsPreview=i==0,ContentType=i==1?LessonContentType.Video:LessonContentType.Text,PublicationStatus=i<3?PublicationStatus.Published:c.Id=="ALG"?PublicationStatus.Draft:PublicationStatus.Scheduled,PublishAtUtc=i==3&&c.Id=="GEO"?DateTime.UtcNow.AddDays(3):null});}}
 var subscriptions=DemoSeedData.Subscriptions.Select(x=>new Subscription{Id="SUB-"+x.Student,StudentId=x.Student,Student=users[x.Student],CourseId=x.Course,Course=courses.Single(c=>c.Id==x.Course),Price=x.Price,StartsAtUtc=DateTime.UtcNow.AddDays(x.EndDays-30),EndsAtUtc=DateTime.UtcNow.AddDays(x.EndDays),Payments=x.Paid==0?[]:[new Payment{Amount=x.Paid,PaidAtUtc=DateTime.UtcNow.AddDays(-2),Method="تحصيل تجريبي"}]}).ToList();
 var groups=courses.Select(c=>new StudyGroup{Id="GROUP-"+c.Id,CourseId=c.Id,Course=c,Name=c.Id=="ALG"?"مجموعة الجبر":"مجموعة الهندسة",Schedule="الأحد والثلاثاء • ٥ مساءً",Students=DemoSeedData.Subscriptions.Where(x=>x.Course==c.Id).Select(x=>new GroupStudent{StudentId=x.Student,Student=users[x.Student]}).ToList()}).ToList();
 var teacher=role=="Teacher";var parent=role=="Parent";
 var p=new DemoPage{Account=users[teacher?"T01":parent?"P01":student],Student=users[student],Students=parent?[users["S01"],users["S02"]]:teacher?users.Values.Where(x=>x.Role==DemoRole.Student).ToList():[users[student]],Courses=teacher?courses:courses.Where(c=>subscriptions.Any(s=>s.StudentId==student&&s.CourseId==c.Id)).ToList(),Groups=groups,Subscriptions=teacher?subscriptions:subscriptions.Where(x=>x.StudentId==student).ToList(),Section=section,Title=section switch{"courses"=>"الكورسات والدروس","groups"=>"المجموعات","subscriptions"=>"الاشتراكات","more"=>"المزيد",_=>teacher?"أهلًا، أستاذ محمد":parent?"متابعة أبنائي":"أهلًا، أحمد"},Subtitle="كل خطوة صغيرة تصنع فرقًا"};return p;
 }
 public IActionResult Index()=>View("/Views/Demo/Welcome.cshtml",Fixture());
 public IActionResult Screen(string role="Teacher",string section="home",string student="S01")=>View("/Views/Demo/Dashboard.cshtml",Fixture(role,section,student));
 public IActionResult Course(string role="Teacher",string student="S01"){var p=Fixture(role,"courses",student);p.Courses=[p.Courses.First()];return View("/Views/Demo/Course.cshtml",p);}
 public IActionResult Group(){var p=Fixture();p.Groups=[p.Groups.First()];return View("/Views/Demo/Group.cshtml",p);}
 public IActionResult Lesson(string role="Student",string student="S01",bool locked=false){var p=Fixture(role,"courses",student);p.Lesson=p.Courses.First().Units.First().Lessons.Last();p.CanOpen=!locked;return View("/Views/Demo/Lesson.cshtml",p);}
 public IActionResult Edit(){ViewData["Page"]=Fixture();ViewData["Title"]="تعديل الدرس";return View("/Views/Demo/Edit.cshtml",new EditLessonModel{Id="ALG-L04",CourseId="ALG",Title="مراجعة الجبر",Description="مراجعة شاملة على المعادلات مع أمثلة وتطبيقات متنوعة.",Status=PublicationStatus.Draft});}
}


