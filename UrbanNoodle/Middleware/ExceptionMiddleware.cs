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
                var requestPath = context.Request.Path;
                var httpMethod = context.Request.Method;


                _logger.LogWarning(ex, "Bad Request tại API {Method} {Path}. Lý do: {Message}",
                    httpMethod, requestPath, ex.Message);

                await HandleError(context, 400, ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy tài nguyên tại {Method} {Path}. Lý do: {Message}",
                context.Request.Method, context.Request.Path, ex.Message);

                await HandleError(context, 404, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Key không tồn tại hệ thống khi gọi {Method} {Path}. Lý do: {Message}",
                context.Request.Method, context.Request.Path, ex.Message);
                await HandleError(context, 404, ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex, "Truy cập trái phép (401) tại {Method} {Path}. Lý do: {Message}",
                context.Request.Method, context.Request.Path, ex.Message);

                await HandleError(context, 401, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Lỗi truy cập trái phép vào tài nguyên người khác (403) xảy ra tại {Method} {Path}. Chi tiết lỗi: {Message}",
               context.Request.Method, context.Request.Path, ex.Message);
                await HandleError(context, 403, "Lỗi truy cập trái phép vào tài nguyên người khác");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi hệ thống nghiêm trọng (500) xảy ra tại {Method} {Path}. Chi tiết lỗi: {Message}",
               context.Request.Method, context.Request.Path, ex.Message);
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
