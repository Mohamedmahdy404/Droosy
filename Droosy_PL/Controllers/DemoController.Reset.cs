using Droosy_DAL.Data;
using Droosy_DAL.Seeding;
using Droosy_PL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Droosy_PL.Controllers;

public partial class DemoController
{
    [HttpPost]
    public async Task<IActionResult> ResetDemo(bool confirmed,
        [FromServices] DemoScope scope,
        [FromServices] DemoRuntime runtime,
        [FromServices] DemoSeeder seeder,
        [FromServices] ILogger<DemoController> logger)
    {
        if (!confirmed) return BadRequest();

        // The visitor middleware holds this visitor's gate and validates the form generation.
        await using (var transaction = await db.Database.BeginTransactionAsync(HttpContext.RequestAborted))
        {
            await VisitorReset.ResetAsync(db);
            await seeder.SeedAsync();
            var state = await db.DemoStates.AsNoTracking().SingleAsync(HttpContext.RequestAborted);
            await transaction.CommitAsync(HttpContext.RequestAborted);
            scope.GenerationId = state.DataGenerationId;
            runtime.Generations[scope.VisitorId] = state.DataGenerationId;
        }

        Response.Cookies.Delete("droosy-account", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("droosy-child", new CookieOptions { Path = "/" });
        try
        {
            UploadStore.ClearVisitor(environment.ContentRootPath, scope.VisitorId);
            TempData["Success"] = "رجّعنا تجربتك للبداية. اختار دورك وابدأ من جديد.";
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(error, "Visitor reset completed but uploaded files could not all be removed.");
            TempData["Error"] = "رجّعنا بيانات تجربتك للبداية، لكن تعذّر حذف بعض المرفقات. جرّب البدء من جديد مرة أخرى.";
        }
        return RedirectToAction(nameof(Index));
    }
}
