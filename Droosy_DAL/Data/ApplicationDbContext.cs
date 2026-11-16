using Droosy_DAL.Entities;
using Microsoft.EntityFrameworkCore;
namespace Droosy_DAL.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
 public DbSet<AppUser> Users => Set<AppUser>();
 public DbSet<ParentStudent> ParentStudents => Set<ParentStudent>();
 public DbSet<PlatformSettings> PlatformSettings => Set<PlatformSettings>();
 public DbSet<AcademicLevel> AcademicLevels => Set<AcademicLevel>();
 public DbSet<Course> Courses => Set<Course>();
 public DbSet<CourseUnit> CourseUnits => Set<CourseUnit>();
 public DbSet<Lesson> Lessons => Set<Lesson>();
 public DbSet<StudyGroup> StudyGroups => Set<StudyGroup>();
 public DbSet<GroupStudent> GroupStudents => Set<GroupStudent>();
 public DbSet<Subscription> Subscriptions => Set<Subscription>();
 public DbSet<Payment> Payments => Set<Payment>();
 public DbSet<ActivationCode> ActivationCodes => Set<ActivationCode>();
 public DbSet<DemoState> DemoStates => Set<DemoState>();
 protected override void OnModelCreating(ModelBuilder model)
 {
  model.Entity<ParentStudent>().HasKey(x => new { x.ParentId, x.StudentId });
  model.Entity<ParentStudent>().HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId);
  model.Entity<ParentStudent>().HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId);
  model.Entity<GroupStudent>().HasKey(x => new { x.GroupId, x.StudentId });
  model.Entity<ActivationCode>().HasIndex(x => x.Code).IsUnique();
  model.Entity<AppUser>().Property(x => x.DisplayName).HasMaxLength(100);
  model.Entity<Lesson>().Property(x => x.Title).HasMaxLength(160);
  model.Entity<Lesson>().Property(x => x.Description).HasMaxLength(2000);
  foreach (var entity in model.Model.GetEntityTypes())
  {
   foreach (var property in entity.GetProperties().Where(p => p.ClrType == typeof(decimal)))
   { property.SetPrecision(12); property.SetScale(2); }
   foreach (var foreignKey in entity.GetForeignKeys()) foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
  }
 }
}
