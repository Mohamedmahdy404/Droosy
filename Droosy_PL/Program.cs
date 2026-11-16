using Droosy_DAL.Data;
using Droosy_DAL.Seeding;
using Droosy_BL;
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
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<LearningService>();
            builder.Services.AddScoped<DemoSeeder>();

            var app = builder.Build();

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

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Demo}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
