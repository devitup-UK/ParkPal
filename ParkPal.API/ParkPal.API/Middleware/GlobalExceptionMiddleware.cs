using Microsoft.AspNetCore.Diagnostics;

namespace ParkPal.API.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Log the actual error so you can debug it later
        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        // 2. Format a clean JSON response for the iOS app
        var problemDetails = new
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server Error",
            Detail = "Something went wrong on our end. Please try again later."
        };

        httpContext.Response.StatusCode = problemDetails.Status;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // 3. Return true to tell ASP.NET "I handled this, don't throw the default HTML error page!"
        return true; 
    }
}