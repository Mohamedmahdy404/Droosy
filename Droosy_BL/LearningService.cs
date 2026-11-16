using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Microsoft.EntityFrameworkCore;
namespace Droosy_BL;
public class LearningService(ApplicationDbContext db)
{
 public IQueryable<Course> Courses => db.Courses.AsNoTracking().Include(x=>x.Level).Include(x=>x.Units).ThenInclude(x=>x.Lessons);
 public async Task<List<Course>> CoursesForAsync(string studentId) => await Courses.Where(c=>db.GroupStudents.Any(g=>g.StudentId==studentId && g.Group.CourseId==c.Id)).ToListAsync();
 public static bool IsPublished(Lesson lesson) => lesson.PublicationStatus==PublicationStatus.Published || (lesson.PublicationStatus==PublicationStatus.Scheduled && lesson.PublishAtUtc<=DateTime.UtcNow);
 public async Task<bool> CanOpenAsync(string studentId, Lesson lesson)
 {
  if (!IsPublished(lesson)) return false;
  var courseId=lesson.Unit.CourseId;
  if (!await db.GroupStudents.AnyAsync(x=>x.StudentId==studentId && x.Group.CourseId==courseId)) return false;
  return lesson.IsPreview || await db.Subscriptions.AnyAsync(x=>x.StudentId==studentId && x.CourseId==courseId && x.StartsAtUtc<=DateTime.UtcNow && x.EndsAtUtc>DateTime.UtcNow);
 }
 public async Task<string?> EditAsync(string id,string title,string description,PublicationStatus status,bool preview,DateTime? publishAt)
 {
  var lesson=await db.Lessons.Include(x=>x.Unit).SingleOrDefaultAsync(x=>x.Id==id);
  if (lesson==null) return null;
  lesson.Title=title.Trim(); lesson.Description=description.Trim(); lesson.PublicationStatus=status; lesson.IsPreview=preview; lesson.PublishAtUtc=status==PublicationStatus.Scheduled?publishAt:null;
  await db.SaveChangesAsync(); return lesson.Unit.CourseId;
 }
}
