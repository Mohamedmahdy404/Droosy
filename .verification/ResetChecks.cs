using Droosy_DAL.Data;
using Droosy_PL.Controllers;
using Droosy_PL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

public static class ResetChecks
{
    public static async Task RunAsync()
    {
        var runtime = new DemoRuntime();
        var visitor = Guid.NewGuid().ToString("N");
        var other = Guid.NewGuid().ToString("N");
        var old = Guid.NewGuid(); var next = Guid.NewGuid(); var otherGeneration = Guid.NewGuid();
        runtime.Generations[visitor] = old; runtime.Generations[other] = otherGeneration;
        var env = new VerificationEnvironment(Directory.GetCurrentDirectory());
        DefaultHttpContext Request(string id, Guid generation)
        {
            var http = new DefaultHttpContext();
            http.Request.Path = "/Demo/ResetDemo";
            http.Request.Method = "POST";
            http.Request.Headers.Cookie = "droosy-visitor=" + id;
            http.Request.ContentType = "application/x-www-form-urlencoded";
            http.Request.Form = new FormCollection(new Dictionary<string,StringValues> { ["demoGeneration"] = generation.ToString() });
            http.Response.Body = new MemoryStream();
            return http;
        }
        var calls = 0;
        var middleware = new DemoVisitorMiddleware(_ => { calls++; return Task.CompletedTask; });
        await middleware.InvokeAsync(Request(visitor,old),new DemoScope(),runtime,env);
        if(calls!=1)throw new Exception("Current generation blocked");
        runtime.Generations[visitor]=next; // Mirrors the committed reset's generation replacement.
        var stale=Request(visitor,old);
        await middleware.InvokeAsync(stale,new DemoScope(),runtime,env);
        if(stale.Response.StatusCode!=409||calls!=1)throw new Exception("Stale tab could save after reset");
        await middleware.InvokeAsync(Request(visitor,next),new DemoScope(),runtime,env);
        await middleware.InvokeAsync(Request(other,otherGeneration),new DemoScope(),runtime,env);
        if(calls!=3||runtime.Generations[other]!=otherGeneration)throw new Exception("Other visitor affected");
        var controller=new DemoController(null!,null!,null!,null!,null!,null!,null!,null!);
        if(await controller.ResetDemo(false,null!,null!,null!,null!) is not BadRequestResult)throw new Exception("Reset accepted without confirmation");
        if(!typeof(DemoController).GetMethod(nameof(DemoController.ResetDemo))!.IsDefined(typeof(HttpPostAttribute),true))throw new Exception("Reset must require POST");
        Console.WriteLine("RESET_CHECKS_PASS: confirmation required, stale-tab rejection, new generation accepted, other visitor unaffected (no SQL used)");
    }
}
