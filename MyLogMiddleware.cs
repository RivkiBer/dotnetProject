using System.Diagnostics;
using System.Threading.Channels;
using BakeryNamespace.Models;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MyMiddleware;

public class MyLogMiddleware
{
    private readonly RequestDelegate next;
    private readonly Channel<LogMessage> _logChannel;

    public MyLogMiddleware(RequestDelegate next, Channel<LogMessage> logChannel)
    {
        this.next = next;
        _logChannel = logChannel;
    }

    public async Task Invoke(HttpContext c)
    {
        var sw = new Stopwatch();
        sw.Start();
        var startTime = DateTime.Now;

        await next.Invoke(c);

        sw.Stop();

        var controller = "";
        var action = "";
        if (c.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>() is { } descriptor)
        {
            controller = descriptor.ControllerName;
            action = descriptor.ActionName;
        }

        var user = c.User?.FindFirst("username")?.Value ?? "unknown";

        var logMessage = new LogMessage
        {
            StartTime = startTime,
            Controller = controller,
            Action = action,
            User = user,
            DurationMs = sw.ElapsedMilliseconds
        };

        await _logChannel.Writer.WriteAsync(logMessage);
    }
}

public static partial class MiddlewareExtensions
{
    public static IApplicationBuilder UseMyLogMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MyLogMiddleware>();
    }
}

