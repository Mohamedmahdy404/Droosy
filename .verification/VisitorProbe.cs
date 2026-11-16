using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Droosy_DAL.Seeding;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
public static class VisitorProbe
{
 public static void Run()
 {
  var options=new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=localhost;Database=Droosy;Integrated Security=True;TrustServerCertificate=True").Options;
  using var a=new ApplicationDbContext(options,new DemoScope{VisitorId="visitor-a"});
  using var b=new ApplicationDbContext(options,new DemoScope{VisitorId="visitor-b"});
  var types=a.Model.GetEntityTypes().ToList();
  if(types.Count!=28 || types.Any(t=>!t.FindPrimaryKey()!.Properties.Any(p=>p.Name=="VisitorId")))throw new Exception("Visitor key coverage");
  if(types.SelectMany(t=>t.GetForeignKeys()).Any(f=>!f.Properties.Any(p=>p.Name=="VisitorId") || !f.PrincipalKey.Properties.Any(p=>p.Name=="VisitorId")))throw new Exception("Unpartitioned foreign key");
  if(!a.Users.ToQueryString().Contains("visitor-a") || !b.Users.ToQueryString().Contains("visitor-b"))throw new Exception("Visitor query filters share first context");
  var reset=File.ReadAllText("Droosy_PL/Services/DemoVisitorMiddleware.cs");
  var order=System.Text.RegularExpressions.Regex.Matches(reset,@"db\.(\w+)\.ExecuteDeleteAsync").Select(x=>x.Groups[1].Value).ToList();
  if(order.Count!=28||order.Distinct().Count()!=28)throw new Exception("Reset coverage");
  foreach(var type in types)
  foreach(var fk in type.GetForeignKeys())
   if(type!=fk.PrincipalEntityType&&order.IndexOf(type.GetTableName()!)>=order.IndexOf(fk.PrincipalEntityType.GetTableName()!))throw new Exception("Reset dependency order: "+type.Name);
  if(reset.Contains("IgnoreQueryFilters"))throw new Exception("Reset bypasses partition");
  try{Droosy_PL.Services.VisitorReset.ResetAsync(a).GetAwaiter().GetResult();throw new Exception("Reset accepted nonvisitor");}catch(InvalidOperationException e)when(e.Message=="Only an individual visitor can be reset."){}
  a.Users.AddRange(new AppUser{Id="T01",DisplayName="teacher"},new AppUser{Id="S01",DisplayName="student"});
  a.AcademicLevels.Add(new AcademicLevel{Id="L1"});
  a.Courses.Add(new Course{Id="C1",TeacherId="T01",LevelId="L1"});
  typeof(ApplicationDbContext).GetMethod("StampVisitor",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(a,null);
  a.ChangeTracker.DetectChanges();
  if(a.ChangeTracker.Entries<IDemoRow>().Any(e=>e.Entity.VisitorId!="visitor-a"))throw new Exception("Visitor stamping");
  Console.WriteLine("VISITOR_MODEL_PASS: 28 entities, composite keys/foreign keys, per-context SQL filters, insert stamps");
 }
}
