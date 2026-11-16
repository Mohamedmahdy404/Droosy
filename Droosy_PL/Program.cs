using Droosy_DAL.Data;
using Droosy_DAL.Seeding;
using Droosy_BL;
using Droosy_PL.Services;
using Microsoft.EntityFrameworkCore;

namespace Droosy_PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<DemoScope>();
            builder.Services.AddSingleton<DemoRuntime>();
            builder.Services.AddScoped<UploadStore>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<LearningService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<AssessmentService>();
            builder.Services.AddScoped<AttendanceService>();
            builder.Services.AddScoped<ProgressService>();
            builder.Services.AddScoped<DemoSeeder>();

            var app = builder.Build();

            var originals=Enumerable.Range(1,4).SelectMany(n=>new[]{"ALG-L0"+n+".pdf","GEO-L0"+n+".pdf"}).Concat(new[]{"ALG-L02.mp4","GEO-L02.mp4"});
            foreach(var file in originals)if(!File.Exists(Path.Combine(app.Environment.ContentRootPath,"App_Data","Lessons",file)))throw new FileNotFoundException("Missing original lesson resource",file);
            if(!File.Exists(Path.Combine(app.Environment.ContentRootPath,"App_Data","SeedAttachments","geometry-solution.png")))throw new FileNotFoundException("Missing original conversation attachment.");
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (!builder.Configuration.GetValue<bool>("Demo:Enabled"))
                    throw new InvalidOperationException("This application requires Demo:Enabled=true.");
                dbContext.Database.Migrate();
                scope.ServiceProvider.GetRequiredService<DemoSeeder>().SeedAsync().GetAwaiter().GetResult();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<DemoVisitorMiddleware>();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Demo}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}



