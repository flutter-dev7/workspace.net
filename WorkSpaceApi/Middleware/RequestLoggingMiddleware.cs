using System;

namespace WorkSpaceApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.Now;
        try
        {
            _logger.LogInformation("Request started: {Method} {Path} at {StartTime}",
            context.Request.Method, context.Request.Path, startTime);

            await _next(context);

            var endTime = DateTime.Now;
            var duration = (endTime - startTime).TotalMilliseconds;

            _logger.LogInformation("Request finished: {Method} {Path} responded {StatusCode} in {Duration} ms",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, duration);
        }
        catch (System.Exception ex)
        {
            var errorTime = DateTime.Now;
            _logger.LogError(ex, "Unhandled exception during request: {Method} {Path} at {ErrorTime}",
            context.Request.Method, context.Request.Path, errorTime);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal Server Error");

        }
    }
}
