using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Microsoft.EntityFrameworkCore;
namespace Droosy_BL;

public class NotificationService(ApplicationDbContext db)
{
 public async Task AddAsync(string user,string? student,string title,string url,string eventKey)
 {
  if(db.Notifications.Local.Any(x=>x.UserId==user&&x.EventKey==eventKey)||await db.Notifications.AnyAsync(x=>x.UserId==user&&x.EventKey==eventKey))return;
  db.Notifications.Add(new Notification{Id=Guid.NewGuid().ToString("N"),UserId=user,StudentId=student,Title=title,TargetUrl=url,EventKey=eventKey,CreatedAtUtc=DateTime.UtcNow});
 }
 public async Task StudentAsync(string student,string title,string url,string eventKey)
 {
  await AddAsync(student,student,title,url,eventKey);
  var parents=await db.ParentStudents.Where(x=>x.StudentId==student).Select(x=>x.ParentId).ToListAsync();
  if(parents.Count==0)return;
  var name=await db.Users.Where(x=>x.Id==student).Select(x=>x.DisplayName).SingleAsync();
  foreach(var parent in parents)await AddAsync(parent,student,name+": "+title,url,eventKey);
 }
}
