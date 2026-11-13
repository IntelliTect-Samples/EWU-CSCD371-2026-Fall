using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var rng = new Random();

app.MapGet("/", () => "MayhemMarmot is awake. Try /file");
app.MapGet("/healthz", () => Results.Ok("ok"));

// Serves a file, but misbehaves on purpose.
// Query params override the random behaviour (handy for demos/tests):
//   ?mode=ok|500|503|404|timeout|slow|truncate|reset   ?seed=123
app.MapGet("/file", async (HttpContext ctx, string? mode, int? seed) =>
{
    var r = seed is int s ? new Random(s) : rng;
    mode ??= r.Next(100) switch
    {
        < 55 => "ok",
        < 65 => "500",
        < 75 => "503",
        < 80 => "404",
        < 87 => "slow",
        < 93 => "truncate",
        < 97 => "reset",
        _ => "timeout",
    };

    ctx.Response.Headers["X-Marmot-Mode"] = mode;
    var payload = Encoding.UTF8.GetBytes(string.Concat(Enumerable.Repeat("marmot ", 5000)));

    switch (mode)
    {
        case "500": return Results.Problem("The marmot chewed the cable.", statusCode: 500);
        case "503":
            ctx.Response.Headers.RetryAfter = "2";
            return Results.StatusCode(503);
        case "404": return Results.NotFound();
        case "timeout":
            await Task.Delay(TimeSpan.FromMinutes(5), ctx.RequestAborted);
            return Results.StatusCode(504);
        case "slow":
            await Task.Delay(TimeSpan.FromSeconds(r.Next(3, 10)), ctx.RequestAborted);
            break;
        case "truncate":
            // Promise full length, send half, then abort the connection.
            ctx.Response.ContentType = "text/plain";
            ctx.Response.ContentLength = payload.Length;
            await ctx.Response.Body.WriteAsync(payload.AsMemory(0, payload.Length / 2), ctx.RequestAborted);
            await ctx.Response.Body.FlushAsync(ctx.RequestAborted);
            ctx.Abort();
            return Results.Empty;
        case "reset":
            ctx.Abort();
            return Results.Empty;
    }

    return Results.File(payload, "text/plain", "marmot.txt");
});

app.Run();
