using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int statusCode;
            string title;
            string detail;

            switch (exception)
            {
                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    title = "Resource Not Found";
                    detail = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status403Forbidden;
                    title = "Access Denied";
                    detail = exception.Message;
                    break;

                case ArgumentException:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = "Invalid Request";
                    detail = exception.Message;
                    break;

                case InvalidOperationException:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = "Business Rule Violation";
                    detail = exception.Message;
                    break;

                case DbUpdateConcurrencyException:
                    statusCode = StatusCodes.Status409Conflict;
                    title = "Data Conflict";
                    detail =
                        "The record was modified by another operation.";
                    break;

                case DbUpdateException:
                    statusCode = StatusCodes.Status409Conflict;
                    title = "Database Conflict";
                    detail =
                        "The requested operation conflicts with existing data.";
                    break;

                default:
                    statusCode =
                        StatusCodes.Status500InternalServerError;

                    title = "Internal Server Error";

                    detail =
                        "An unexpected error occurred. Please try again.";
                    break;
            }

            if (statusCode >= 500)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception occurred. TraceId: {TraceId}",
                    httpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning(
                    exception,
                    "Request failed with status {StatusCode}. TraceId: {TraceId}",
                    statusCode,
                    httpContext.TraceIdentifier);
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["traceId"] =
                httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }
    }
}