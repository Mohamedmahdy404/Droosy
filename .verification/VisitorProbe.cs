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
  a.Users.AddRange(new AppUser{Id="T01",DisplayName="teacher"},new AppUser{Id="S01",DisplayName="student"});
  a.AcademicLevels.Add(new AcademicLevel{Id="L1"});
  a.Courses.Add(new Course{Id="C1",TeacherId="T01",LevelId="L1"});
  typeof(ApplicationDbContext).GetMethod("StampVisitor",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(a,null);
  a.ChangeTracker.DetectChanges();
  if(a.ChangeTracker.Entries<IDemoRow>().Any(e=>e.Entity.VisitorId!="visitor-a"))throw new Exception("Visitor stamping");
  Console.WriteLine("VISITOR_MODEL_PASS: 28 entities, composite keys/foreign keys, per-context SQL filters, insert stamps");
 }
}
