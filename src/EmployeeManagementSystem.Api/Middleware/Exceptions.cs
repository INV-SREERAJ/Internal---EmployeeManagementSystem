using EmployeeManagementSystem.Business.GlobalExceptionHandler;
using System.Text.Json;

namespace EmployeeManagementSystem.Api.Middleware
{
    public class Exceptions
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<Exceptions> _logger;
        public Exceptions(RequestDelegate next, ILogger<Exceptions> logger)
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
            catch (ConflictException ex)
            {
                await HandleExceptionAsync(
                    context,
                    StatusCodes.Status409Conflict,
                    ex.Message);
            }
            catch(NotFoundException ex)
            {
                await HandleExceptionAsync(
                    context,
                    StatusCodes.Status404NotFound, ex.Message);
            }
            catch(UnAuthorizedException ex)
            {
                await HandleExceptionAsync(
                        context,
                        StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError( ex,
                        "Unhandled exception while processing {Method} {Path}",
                        context.Request.Method,
                        context.Request.Path);

                await HandleExceptionAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            int statusCode,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                StatusCode = statusCode,
                Message = message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
