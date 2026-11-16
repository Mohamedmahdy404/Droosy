using System.Collections.Concurrent;
using Droosy_DAL.Data;
using Droosy_DAL.Seeding;
using Microsoft.EntityFrameworkCore;
namespace Droosy_PL.Services;
public sealed class DemoRuntime
{
 public Guid RuntimeId {get;}=Guid.NewGuid();
 public ConcurrentDictionary<string,Guid> Generations {get;}=new();
 public ConcurrentDictionary<string,SemaphoreSlim> Gates {get;}=new();
}
public static class VisitorReset
{
 public static async Task ResetAsync(ApplicationDbContext db)
 {
  if(!Guid.TryParseExact(db.VisitorId,"N",out _))throw new InvalidOperationException("Only an individual visitor can be reset.");
  await db.Notifications.ExecuteDeleteAsync();await db.Messages.ExecuteDeleteAsync();await db.Conversations.ExecuteDeleteAsync();await db.Announcements.ExecuteDeleteAsync();
  await db.ExamAnswers.ExecuteDeleteAsync();await db.ExamAttempts.ExecuteDeleteAsync();await db.ExamChoices.ExecuteDeleteAsync();await db.ExamQuestions.ExecuteDeleteAsync();await db.Exams.ExecuteDeleteAsync();
  await db.AssignmentSubmissions.ExecuteDeleteAsync();await db.Assignments.ExecuteDeleteAsync();await db.LessonProgresses.ExecuteDeleteAsync();await db.Attendances.ExecuteDeleteAsync();await db.AuditLogs.ExecuteDeleteAsync();
  await db.ClassSessions.ExecuteDeleteAsync();await db.ActivationCodes.ExecuteDeleteAsync();await db.Payments.ExecuteDeleteAsync();await db.Subscriptions.ExecuteDeleteAsync();await db.GroupStudents.ExecuteDeleteAsync();
  await db.StudyGroups.ExecuteDeleteAsync();await db.Lessons.ExecuteDeleteAsync();await db.CourseUnits.ExecuteDeleteAsync();await db.Courses.ExecuteDeleteAsync();await db.ParentStudents.ExecuteDeleteAsync();
  await db.PlatformSettings.ExecuteDeleteAsync();await db.AcademicLevels.ExecuteDeleteAsync();await db.Users.ExecuteDeleteAsync();await db.DemoStates.ExecuteDeleteAsync();
  db.ChangeTracker.Clear();
 }
}
public class DemoVisitorMiddleware(RequestDelegate next)
{
 public async Task InvokeAsync(HttpContext context,DemoScope scope,DemoRuntime runtime,IWebHostEnvironment environment)
 {
  if(!context.Request.Path.StartsWithSegments("/Demo")&&context.Request.Path!="/"){await next(context);return;}
  var cookie=context.Request.Cookies["droosy-visitor"];
  var visitor=Guid.TryParseExact(cookie,"N",out var existing)?existing.ToString("N"):Guid.NewGuid().ToString("N");
  scope.VisitorId=visitor;scope.RuntimeId=runtime.RuntimeId;
  if(cookie!=visitor)context.Response.Cookies.Append("droosy-visitor",visitor,new CookieOptions{IsEssential=true,Path="/",SameSite=SameSiteMode.Lax,MaxAge=TimeSpan.FromDays(30)});
  var gate=runtime.Gates.GetOrAdd(visitor,_=>new SemaphoreSlim(1,1));
  await gate.WaitAsync(context.RequestAborted);
  try
  {
   if(!runtime.Generations.TryGetValue(visitor,out var generation))
   {
    var db=context.RequestServices.GetRequiredService<ApplicationDbContext>();
    var state=await db.DemoStates.AsNoTracking().SingleOrDefaultAsync();
    await using var transaction=await db.Database.BeginTransactionAsync(context.RequestAborted);
    if(state!=null&&state.RuntimeId!=runtime.RuntimeId)await VisitorReset.ResetAsync(db);
    await context.RequestServices.GetRequiredService<DemoSeeder>().SeedAsync();
    state=await db.DemoStates.AsNoTracking().SingleAsync();
    await transaction.CommitAsync(context.RequestAborted);
    UploadStore.ClearVisitor(environment.ContentRootPath,visitor);
    generation=state.DataGenerationId;runtime.Generations[visitor]=generation;
   }
   scope.GenerationId=generation;
   if(HttpMethods.IsPost(context.Request.Method))
   {
    string? supplied=context.Request.Headers["X-Droosy-Generation"].FirstOrDefault();
    if(context.Request.HasFormContentType)supplied=(await context.Request.ReadFormAsync(context.RequestAborted))["demoGeneration"].FirstOrDefault()??supplied;
    if(!Guid.TryParse(supplied,out var expected)||expected!=generation)
    {
     context.Response.StatusCode=409;
     if(context.Request.HasFormContentType){context.Response.ContentType="text/html; charset=utf-8";await context.Response.WriteAsync("<!doctype html><html lang='ar' dir='rtl'><meta name='viewport' content='width=device-width,initial-scale=1'><body style='background:#f7f4ea;color:#163f33;font-family:Tahoma;padding:30px;line-height:2'><h1>نبدأ خطوة جديدة</h1><p>بدأت دورة جديدة للتجربة. افتح الصفحة من جديد قبل حفظ التغييرات.</p><a href='/Demo'>العودة إلى دروسي ←</a></body></html>");}
     else await context.Response.WriteAsJsonAsync(new{error="بدأت دورة جديدة للتجربة. أعد فتح الصفحة.",reload=true});
     return;
    }
   }
   await next(context);
  }
  finally{gate.Release();}
 }
}
