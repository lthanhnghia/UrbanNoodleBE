using System.Text.Json;
using UrbanNoodle.Dto;
using UrbanNoodle.Exceptions;

namespace UrbanNoodle.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex.Message);

                await HandleError(context, 400, ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);

                await HandleError(context, 404, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                await HandleError(context, 404, ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex.Message);

                await HandleError(context, 401, ex.Message);
            }
            catch (Exception ex)
            {
                 _logger.LogWarning(ex.Message);
                await HandleError(context, 500, "Lỗi hệ thống");
            }
           
        }

        private async Task HandleError(
            HttpContext context,
            int statusCode,
            string message)
        {
            context.Response.StatusCode = statusCode;

            context.Response.ContentType = "application/json";

            var error = new ApiResponse(statusCode, message);

            var json = JsonSerializer.Serialize(error);

            await context.Response.WriteAsync(json);
        }
    }
}
