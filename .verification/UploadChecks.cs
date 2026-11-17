using Droosy_DAL.Data;
using Droosy_PL.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

public sealed class VerificationEnvironment(string root):IWebHostEnvironment
{
 public string ApplicationName{get;set;}="Droosy_PL";public string EnvironmentName{get;set;}="Development";
 public string ContentRootPath{get;set;}=root;public IFileProvider ContentRootFileProvider{get;set;}=new NullFileProvider();
 public string WebRootPath{get;set;}="";public IFileProvider WebRootFileProvider{get;set;}=new NullFileProvider();
}
public static class UploadChecks
{
 public static async Task RunAsync()
 {
  var root=Path.Combine(Directory.GetCurrentDirectory(),".verification","upload-check");Directory.CreateDirectory(root);
  var a=Guid.NewGuid().ToString("N");var b=Guid.NewGuid().ToString("N");var scope=new DemoScope{VisitorId=a,GenerationId=Guid.NewGuid()};var store=new UploadStore(new VerificationEnvironment(root),scope);
  var png=File.ReadAllBytes("Droosy_PL/App_Data/SeedAttachments/geometry-solution.png");
  using var input=new MemoryStream(png);var file=new FormFile(input,0,png.Length,"file","solution.png");var saved=await store.SaveAsync(file);
  if(saved==null||!File.Exists(store.Resolve(saved.Path)))throw new Exception("Upload did not persist");
  bool blocked=false;try{store.Resolve($"Uploads/{a}/../{b}/secret.png");}catch(ArgumentException){blocked=true;}if(!blocked)throw new Exception("Visitor upload traversal allowed");
  blocked=false;try{store.Resolve($"Uploads/{b}/secret.png");}catch(ArgumentException){blocked=true;}if(!blocked)throw new Exception("Other visitor attachment accessible");
  using var bad=new MemoryStream("plain text"u8.ToArray());blocked=false;try{await store.SaveAsync(new FormFile(bad,0,bad.Length,"file","bad.pdf"));}catch(ArgumentException){blocked=true;}if(!blocked)throw new Exception("Invalid file accepted");
  using var empty=new MemoryStream();if(await store.SaveAsync(new FormFile(empty,0,0,"file","empty.pdf"))!=null)throw new Exception("Empty upload accepted");
  var boundary=new byte[UploadStore.ChatFileMaxBytes];png.AsSpan(0,8).CopyTo(boundary);
  for(var n=0;n<5;n++){using var payload=new MemoryStream(boundary);await store.SaveAsync(new FormFile(payload,0,payload.Length,"file","chat.png"),chat:true);}
  if(store.ChatUsedBytes()!=UploadStore.ChatQuotaBytes)throw new Exception("Shared chat quota accounting");
  using(var payload=new MemoryStream(png))
  {blocked=false;try{await store.SaveAsync(new FormFile(payload,0,payload.Length,"file","overflow.png"),chat:true);}catch(ArgumentException){blocked=true;}if(!blocked)throw new Exception("Chat exceeded shared 10 MB quota");}
  using(var payload=new MemoryStream(new byte[UploadStore.ChatFileMaxBytes+1]))
  {blocked=false;try{await store.SaveAsync(new FormFile(payload,0,payload.Length,"file","large.png"),chat:true);}catch(ArgumentException){blocked=true;}if(!blocked)throw new Exception("Chat accepted attachment above 2 MB");}
  var otherStore=new UploadStore(new VerificationEnvironment(root),new DemoScope{VisitorId=b,GenerationId=scope.GenerationId});
  if(otherStore.ChatUsedBytes()!=0)throw new Exception("Chat quota crossed visitor boundary");
  var originals=Path.Combine(root,"App_Data","Lessons");Directory.CreateDirectory(originals);var original=Path.Combine(originals,"original.pdf");await File.WriteAllTextAsync(original,"original");
  var other=Path.Combine(root,"App_Data","Uploads",b);Directory.CreateDirectory(other);await File.WriteAllTextAsync(Path.Combine(other,"keep.txt"),"other visitor");
  UploadStore.ClearVisitor(root,a);
  if(File.Exists(store.Resolve(saved.Path))||!File.Exists(original)||!File.Exists(Path.Combine(other,"keep.txt")))throw new Exception("Visitor reset crossed file boundary");
  blocked=false;try{UploadStore.ClearVisitor(root,"template");}catch(InvalidOperationException){blocked=true;}if(!blocked)throw new Exception("Template folder cleanup allowed");
  Console.WriteLine("UPLOAD_CHECKS_PASS: actual image save, invalid/empty file rejection, visitor path isolation, visitor-only cleanup, original assets preserved");
 }
}
