using Microsoft.AspNetCore.Mvc;

using MiniLMS.Application.Exceptions;

namespace MiniLMS.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var (status, title, detail) = ex switch
                {
                    ArgumentException => (StatusCodes.Status400BadRequest, "Bad request", ex.Message),
                    KeyNotFoundException => (StatusCodes.Status404NotFound, "Not found", ex.Message),
                    ConflictException => (StatusCodes.Status409Conflict, "Conflict", ex.Message),
                    _ => (StatusCodes.Status500InternalServerError, "Server error",
                          "An unexpected error occurred. Please try again later.")
                };

                if (status == StatusCodes.Status500InternalServerError)
                    _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                        context.Request.Method, context.Request.Path);
                else
                    _logger.LogWarning("{Status} on {Method} {Path}: {Message}",
                        status, context.Request.Method, context.Request.Path, ex.Message);

                var problem = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = detail,
                    Instance = context.Request.Path
                };
                problem.Extensions["traceId"] = context.TraceIdentifier;

                context.Response.StatusCode = status;
                await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
            }
        }

    }
}
