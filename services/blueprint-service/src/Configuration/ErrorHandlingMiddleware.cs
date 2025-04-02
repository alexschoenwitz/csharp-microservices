using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BlueprintService.Configuration
    {
    public class ErrorHandlingMiddleware
        {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
            {
            _next = next;
            }

        public async Task InvokeAsync(HttpContext context)
            {
            try
                {
                await _next(context).ConfigureAwait(false);
                }
            catch (Exception ex)
                {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
                }
            }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";

            // Customize response based on exception type
            switch (exception)
                {
                case ArgumentException _:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                case InvalidOperationException _:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                case UnauthorizedAccessException _:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Unauthorized access";
                    break;
                default:
                    // Log the unexpected exception
                    Console.Error.WriteLine($"Unhandled exception: {exception}");
                    break;
                }

            // Set the response details
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
                {
                status = (int)statusCode,
                message,
                traceId = context.TraceIdentifier
                };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response)).ConfigureAwait(false);
            }
        }
    }
