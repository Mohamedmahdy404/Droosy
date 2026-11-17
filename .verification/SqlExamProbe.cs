using Droosy_BL;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Droosy_DAL.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public static class SqlExamProbe
{
 public static async Task RunAsync()
 {
  var config=new ConfigurationBuilder().AddJsonFile(Path.GetFullPath("Droosy_PL/appsettings.json")).Build();
  var connection=new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(config.GetConnectionString("DefaultConnection")){ConnectTimeout=8};
  var scope=new DemoScope{VisitorId=Guid.NewGuid().ToString("N"),RuntimeId=Guid.NewGuid()};
  await using var db=new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connection.ConnectionString).Options,scope);
  try{
   var migrations=(await db.Database.GetAppliedMigrationsAsync()).ToList();
   Console.WriteLine("SQL_CONNECTED: migrations="+migrations.Count);
   if(!migrations.Any(x=>x.Contains("VisitorIsolation"))){Console.WriteLine("SQL_SCHEMA_MISSING: no changes applied");return;}
   await using var transaction=await db.Database.BeginTransactionAsync();
   try{
    await new DemoSeeder(db).SeedAsync();
    var notifications=new NotificationService(db);var service=new AssessmentService(db,notifications);
    var exam=await db.Exams.Include(x=>x.Questions).ThenInclude(x=>x.Choices).SingleAsync(x=>x.Id=="ALG-X04");
    var attempt=await service.StartAsync(exam,"S01");
    var selected=exam.Questions.ToDictionary(q=>q.Id,q=>q.Choices.First().Id);
    var error=await service.SaveAnswersAsync(attempt,selected,false);
    Console.WriteLine("SQL_AUTOSAVE: "+(error??"OK"));
    error=await service.SaveAnswersAsync(attempt,selected,true);
    db.ChangeTracker.Clear();
    var saved=await db.ExamAttempts.Include(x=>x.Answers).SingleAsync(x=>x.ExamId==exam.Id&&x.StudentId=="S01");
    if(error!=null||saved.SubmittedAtUtc==null||saved.Answers.Count!=5)throw new Exception("Submission did not persist");
    Console.WriteLine("SQL_EXAM_PASS: autosave, final submit, five persisted answers");
   }finally{await transaction.RollbackAsync();Console.WriteLine("SQL_PROBE_ROLLED_BACK: isolated test visitor, no committed rows");}
  }catch(Exception e){for(var current=e;current!=null;current=current.InnerException!)Console.WriteLine(current.GetType().Name+": "+current.Message);}
 }
}
