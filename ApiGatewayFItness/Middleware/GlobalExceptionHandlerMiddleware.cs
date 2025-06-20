// Api/Middleware/GlobalExceptionHandlerMiddleware.cs (Hoặc một thư mục Common/Middleware)
using Domain.Exceptions; // Namespace của ValidationException (tùy chỉnh)
using Application.Responses;        // Namespace của Result<T>
using Application.Responses.Interfaces; // Namespace của IResult<T>
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting; // Cần cho IHostEnvironment (để biết là Development hay không)
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic; // Cần cho List<string>
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
// Thêm using cho các custom exception khác nếu có
// using Domain.Exceptions;

namespace Api.Middleware; // Namespace phù hợp với project API của bạn

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env; // Để kiểm tra môi trường (Development/Production)

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment env) // Inject IHostEnvironment
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

   

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Chuyển request cho middleware tiếp theo trong pipeline
        }
        catch (Exception ex)
        {
            // Ghi log lỗi với đầy đủ thông tin
            _logger.LogError(ex, "An unhandled exception has occurred. Request Path: {Path}, Method: {Method}",
                             context.Request.Path, context.Request.Method);

            // Xử lý và tạo response lỗi
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        IResult<object?> errorResult; // Dùng object? cho Data vì nó sẽ là null
        HttpStatusCode statusCode;

        switch (exception)
        {
            // Lỗi Validation từ FluentValidation (ném ra từ ValidationBehavior)
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest; // 400
                var validationMessages = validationException.Errors
                    .SelectMany(pair => pair.Select(msg => $"{pair}: {msg}"))
                    .ToList();
                // Hoặc nếu muốn trả về dictionary lỗi:
                // var errorsDict = validationException.Errors;
                // errorResult = Result<object?>.Failure(errorsDict, (int)statusCode); // Cần sửa Result.Failure để nhận Dictionary
                errorResult = Result<object?>.Failure(validationMessages, (int)statusCode);
                break;

            // Lỗi không tìm thấy (tạo custom exception này trong Domain hoặc Application)
            // Ví dụ: public class NotFoundException : Exception { public NotFoundException(string message) : base(message) {} }
            case NotFoundException notFoundException: // Giả sử bạn có NotFoundException
                statusCode = HttpStatusCode.NotFound; // 404
                errorResult = Result<object?>.Failure(notFoundException.Message, (int)statusCode);
                break;

            // Lỗi xác thực (khi token không hợp lệ hoặc thiếu)
            // Thường JwtBearerEvents.OnChallenge sẽ xử lý, nhưng đây là fallback nếu exception ném ra từ code khác
            case UnauthorizedAccessException unauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized; // 401
                errorResult = Result<object?>.Unauthorized();
                _logger.LogWarning(unauthorizedAccessException, "Unauthorized access attempt.");
                break;

            // Lỗi ủy quyền (khi token hợp lệ nhưng user không có quyền)
            // Thường JwtBearerEvents.OnForbidden sẽ xử lý, nhưng đây là fallback
            // Bạn nên tạo một Custom ForbiddenException
            // Ví dụ: public class ForbiddenAccessException : Exception { public ForbiddenAccessException() : base() {} }
            case ForbiddenException forbiddenAccessException: // Giả sử bạn có ForbiddenAccessException
                statusCode = HttpStatusCode.Forbidden; // 403
                errorResult = Result<object?>.Forbidden();
                _logger.LogWarning(forbiddenAccessException, "Forbidden access attempt.");
                break;

            // Các lỗi nghiệp vụ cụ thể khác (tạo các custom exception riêng)
            // case YourSpecificBusinessLogicException specificEx:
            //     statusCode = HttpStatusCode.Conflict; // 409 (Ví dụ: tạo tài nguyên đã tồn tại)
            //     errorResult = Result<object?>.Failure(specificEx.Message, (int)statusCode);
            //     break;

            // Lỗi chung không mong muốn
            default:
                statusCode = HttpStatusCode.InternalServerError; // 500
                // Trong môi trường Production, không nên lộ chi tiết lỗi
                var message = _env.IsDevelopment()
                    ? $"An unexpected error occurred: {exception.Message} \nStackTrace: {exception.StackTrace}"
                    : "An unexpected internal server error has occurred. Please try again later.";
                errorResult = Result<object?>.Failure(message, (int)statusCode);
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // output: camelCase
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull // Bỏ qua trường null
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResult, options));
    }
}

