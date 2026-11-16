using Droosy_DAL.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
namespace Droosy_DAL.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, DemoScope? scope = null) : DbContext(options)
{
 public string VisitorId => scope?.VisitorId ?? "template";
 public Guid RuntimeId => scope?.RuntimeId ?? Guid.Empty;
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
 public DbSet<ClassSession> ClassSessions => Set<ClassSession>();
 public DbSet<Attendance> Attendances => Set<Attendance>();
 public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
 public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
 public DbSet<Assignment> Assignments => Set<Assignment>();
 public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();
 public DbSet<Exam> Exams => Set<Exam>();
 public DbSet<ExamQuestion> ExamQuestions => Set<ExamQuestion>();
 public DbSet<ExamChoice> ExamChoices => Set<ExamChoice>();
 public DbSet<ExamAttempt> ExamAttempts => Set<ExamAttempt>();
 public DbSet<ExamAnswer> ExamAnswers => Set<ExamAnswer>();
 public DbSet<Conversation> Conversations => Set<Conversation>();
 public DbSet<Message> Messages => Set<Message>();
 public DbSet<Announcement> Announcements => Set<Announcement>();
 public DbSet<Notification> Notifications => Set<Notification>();
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
  model.Entity<Attendance>().HasIndex(x => new { x.SessionId, x.StudentId }).IsUnique();
  model.Entity<LessonProgress>().HasIndex(x => new { x.StudentId, x.LessonId }).IsUnique();
  model.Entity<Attendance>().Property(x => x.Note).HasMaxLength(300);
  model.Entity<Attendance>().Property(x => x.PublishedNote).HasMaxLength(300);
  model.Entity<AssignmentSubmission>().HasIndex(x => new { x.AssignmentId, x.StudentId }).IsUnique();
  model.Entity<ExamAttempt>().HasIndex(x => new { x.ExamId, x.StudentId }).IsUnique();
  model.Entity<ExamAnswer>().HasIndex(x => new { x.AttemptId, x.QuestionId }).IsUnique();
  model.Entity<Notification>().HasIndex(x => new { x.UserId, x.EventKey }).IsUnique();
  foreach (var entity in model.Model.GetEntityTypes())
  {
   foreach (var property in entity.GetProperties().Where(p => p.ClrType == typeof(decimal)))
   { property.SetPrecision(12); property.SetScale(2); }
   foreach (var foreignKey in entity.GetForeignKeys()) foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
  }
  ConfigureVisitors(model);
 }
 private void ConfigureVisitors(ModelBuilder model)
 {
  var entities=model.Model.GetEntityTypes().Where(x=>typeof(IDemoRow).IsAssignableFrom(x.ClrType)).ToArray();
  var relations=entities.SelectMany(e=>e.GetForeignKeys().Select(f=>new {
   Dependent=e.ClrType,Principal=f.PrincipalEntityType.ClrType,Properties=f.Properties.Select(p=>p.Name).ToArray(),
   PrincipalProperties=f.PrincipalKey.Properties.Select(p=>p.Name).ToArray(),DependentNav=f.DependentToPrincipal?.Name,
   PrincipalNav=f.PrincipalToDependent?.Name,Required=f.IsRequired})).ToArray();
  foreach(var entity in entities)
  {
   var keyNames=entity.FindPrimaryKey()!.Properties.Select(p=>p.Name).Where(n=>n!="VisitorId").ToArray();
   model.Entity(entity.ClrType).Property<string>("VisitorId").HasMaxLength(64).IsUnicode(false).HasDefaultValue("template");
   foreach(var property in entity.GetProperties().Where(p=>p.ClrType==typeof(string)&&p.Name!="VisitorId"&&(p.Name=="Id"||p.Name.EndsWith("Id"))))property.SetMaxLength(160);
   model.Entity(entity.ClrType).HasKey(new[]{"VisitorId"}.Concat(keyNames).ToArray());
  }
  foreach(var relation in relations)
   model.Entity(relation.Dependent).HasOne(relation.Principal,relation.DependentNav).WithMany(relation.PrincipalNav)
    .HasForeignKey(new[]{"VisitorId"}.Concat(relation.Properties.Where(n=>n!="VisitorId")).ToArray())
    .HasPrincipalKey(new[]{"VisitorId"}.Concat(relation.PrincipalProperties.Where(n=>n!="VisitorId")).ToArray())
    .IsRequired(relation.Required).OnDelete(DeleteBehavior.Restrict);
  foreach(var entity in entities)
  {
   foreach(var key in entity.GetKeys().Where(k=>k!=entity.FindPrimaryKey()&&!k.GetReferencingForeignKeys().Any()).ToArray())entity.RemoveKey(key);
   var indices=entity.GetIndexes().Select(i=>new {Names=i.Properties.Select(p=>p.Name).Where(n=>n!="VisitorId").ToArray(),Unique=i.IsUnique}).ToList();
   foreach(var index in entity.GetIndexes().ToArray())entity.RemoveIndex(index);
   foreach(var index in indices)model.Entity(entity.ClrType).HasIndex(new[]{"VisitorId"}.Concat(index.Names).ToArray()).IsUnique(index.Unique);
   var parameter=Expression.Parameter(entity.ClrType,"row");
   entity.SetQueryFilter(Expression.Lambda(Expression.Equal(Expression.Property(parameter,"VisitorId"),Expression.Property(Expression.Constant(this),nameof(VisitorId))),parameter));
  }
 }
 private void StampVisitor()
 {
  foreach(var entry in ChangeTracker.Entries<IDemoRow>())
  {
   if(entry.State==EntityState.Added)entry.Entity.VisitorId=VisitorId;
   else if(entry.State is EntityState.Modified or EntityState.Deleted && entry.Entity.VisitorId!=VisitorId)
    throw new InvalidOperationException("A different visitor's row cannot be changed.");
   if(entry.State==EntityState.Added && entry.Entity is DemoState state)state.RuntimeId=RuntimeId;
  }
 }
 public override int SaveChanges(bool acceptAllChangesOnSuccess){StampVisitor();return base.SaveChanges(acceptAllChangesOnSuccess);}
 public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,CancellationToken cancellationToken=default){StampVisitor();return base.SaveChangesAsync(acceptAllChangesOnSuccess,cancellationToken);}
}


