using Droosy_DAL.Data;
namespace Droosy_PL.Services;
public record SavedUpload(string Path,string Name,string ContentType);
public class UploadStore(IWebHostEnvironment environment,DemoScope scope)
{
 public const long MaxBytes=5*1024*1024;
 public async Task<SavedUpload?> SaveAsync(IFormFile? file)
 {
  if(file==null||file.Length==0)return null;
  if(file.Length>MaxBytes)throw new ArgumentException("حجم المرفق يجب ألا يزيد عن ٥ ميجابايت.");
  var header=new byte[8];await using(var stream=file.OpenReadStream()){await stream.ReadExactlyAsync(header.AsMemory(0,(int)Math.Min(8,file.Length)));}
  string extension,type;
  if(header[0]==0x25&&header[1]==0x50&&header[2]==0x44&&header[3]==0x46){extension=".pdf";type="application/pdf";}
  else if(header.SequenceEqual(new byte[]{137,80,78,71,13,10,26,10})){extension=".png";type="image/png";}
  else if(header[0]==255&&header[1]==216&&header[2]==255){extension=".jpg";type="image/jpeg";}
  else throw new ArgumentException("اختر صورة PNG أو JPG أو ملف PDF.");
  var relative=$"Uploads/{scope.VisitorId}/{scope.GenerationId:N}/{Guid.NewGuid():N}{extension}";
  var full=Resolve(relative);Directory.CreateDirectory(System.IO.Path.GetDirectoryName(full)!);
  await using(var target=new FileStream(full,FileMode.CreateNew,FileAccess.Write))await file.CopyToAsync(target);
  var name=System.IO.Path.GetFileName(file.FileName);return new(relative,string.IsNullOrEmpty(name)?"مرفق"+extension:name,type);
 }
 public string Resolve(string relative)
 {
  var allowed=relative.StartsWith($"Uploads/{scope.VisitorId}/",StringComparison.Ordinal)||relative.StartsWith("Lessons/",StringComparison.Ordinal)||relative.StartsWith("SeedAttachments/",StringComparison.Ordinal);
  var root=System.IO.Path.GetFullPath(System.IO.Path.Combine(environment.ContentRootPath,"App_Data"));
  var path=System.IO.Path.GetFullPath(System.IO.Path.Combine(root,relative));
  var folder=relative.StartsWith("Uploads/",StringComparison.Ordinal)?System.IO.Path.Combine(root,"Uploads",scope.VisitorId):relative.StartsWith("Lessons/",StringComparison.Ordinal)?System.IO.Path.Combine(root,"Lessons"):System.IO.Path.Combine(root,"SeedAttachments");
  var boundary=System.IO.Path.GetFullPath(folder)+System.IO.Path.DirectorySeparatorChar;
  if(!allowed||!path.StartsWith(boundary,StringComparison.OrdinalIgnoreCase))throw new ArgumentException("المرفق غير متاح لهذه التجربة.");
  return path;
 }
 public void Delete(string? relative)
 {
  if(relative==null||!relative.StartsWith($"Uploads/{scope.VisitorId}/",StringComparison.Ordinal))return;
  var path=Resolve(relative);if(File.Exists(path))File.Delete(path);
 }
 public static void ClearVisitor(string contentRoot,string visitor)
 {
  if(!Guid.TryParseExact(visitor,"N",out _))throw new InvalidOperationException("Invalid visitor folder.");
  var root=System.IO.Path.GetFullPath(System.IO.Path.Combine(contentRoot,"App_Data","Uploads"));
  var target=System.IO.Path.GetFullPath(System.IO.Path.Combine(root,visitor));
  if(!target.StartsWith(root+System.IO.Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Invalid cleanup path.");
  if(Directory.Exists(target))Directory.Delete(target,true);
 }
}

