// using ApiGatewayFitness.Middleware; // Namespace của TokenLoggingMiddleware (xem lại nếu cần)
using Application.Responses; // Namespace của Result<T>
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging; // Thêm using cho ILogger
using ApiGatewayFitness.Middleware;
using Api.Middleware;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 1. Load Configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
// Không cần dòng này nữa vì builder.Configuration đã bao gồm appsettings.json
// builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Configuration; // Dùng trực tiếp builder.Configuration

// 2. Lấy cấu hình JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Key"]; // Đổi "Key" thành "Secret" cho nhất quán với ví dụ trước
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("JWT settings (Secret, Issuer, Audience) are not configured properly in appsettings.json for API Gateway.");
}

// 3. Add Ocelot services
builder.Services.AddOcelot(); // Gọi trước AddAuthentication nếu Ocelot có các tùy chọn liên quan đến Auth

// 4. Add Authentication (JWT Bearer) for Gateway
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    // options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // Dòng này thường không cần thiết khi 2 dòng trên đã set
})
.AddJwtBearer("FitnessAppBearer", options => // Key này phải khớp với AuthenticationProviderKey trong ocelot.json
{
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.SaveToken = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true, // QUAN TRỌNG: Xác thực token còn hạn
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Không cho phép sai lệch thời gian
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
            var token = context.Token;
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                 token = authorizationHeader.Substring("Bearer ".Length).Trim();

            }
                logger.LogDebug("API Gateway - OnMessageReceived - Authorization Header: {AuthHeader}", authorizationHeader);
            if (!string.IsNullOrEmpty(token))
            {

                logger.LogDebug("API Gateway - Token received: {TokenStart}", token.Length > 20 ? token.Substring(0, 20) + "..." : token);
            }
            else
            {
                logger.LogDebug("API Gateway - No token received in request.");
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
            logger.LogInformation("API Gateway - Token Validated successfully for User: {User}", context.Principal?.Identity?.Name ?? "Unknown");
            // Log claims để debug
            // foreach (var claim in context.Principal.Claims)
            // {
            //     logger.LogDebug("Claim Type: {ClaimType}, Value: {ClaimValue}", claim.Type, claim.Value);
            // }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            // Lỗi xảy ra trong quá trình xác thực (ví dụ: chữ ký sai, token malformed, hết hạn)
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
            logger.LogError(context.Exception, "API Gateway - Authentication Failed.");


            //// Ngăn chặn hành vi mặc định (quan trọng!)

            //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //context.Response.ContentType = "application/json";

            //var errorResponse = Result<object?>.Unauthorized(); // Dùng object? hoặc string nếu Data chỉ là string message

            // context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Đảm bảo nhất quán casing
            //    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull // Không serialize các thuộc tính null
            //}));
            // Không cần set response ở đây, OnChallenge sẽ xử lý
            return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {
            // Được gọi khi xác thực thất bại VÀ endpoint yêu cầu xác thực.
            // Đây là nơi chúng ta custom response 401.
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
            logger.LogWarning("API Gateway - JWT Challenge triggered. AuthenticationFailure: {FailureMessage}", context.AuthenticateFailure?.Message ?? "No specific failure message.");

            // Ngăn chặn hành vi mặc định (quan trọng!)
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var errorResponse = Result<object?>.Unauthorized(); // Dùng object? hoặc string nếu Data chỉ là string message

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Đảm bảo nhất quán casing
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull // Không serialize các thuộc tính null
            }));
        },
        OnForbidden = async context =>
        {
            // Được gọi khi user đã xác thực nhưng không có quyền truy cập (ví dụ: RouteClaimsRequirement của Ocelot thất bại)
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
            logger.LogWarning("API Gateway - JWT Forbidden triggered for User: {User}", context.Principal?.Identity?.Name ?? "Unknown");

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var errorResponse = Result<object?>.Forbidden();
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }));
        }
    };
});

// (Tùy chọn) CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutterApp", policy =>
    {
        policy.WithOrigins("http://localhost", "https://your-flutter-app-domain.com") // Cập nhật domain
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Không cần AddControllers() nếu Gateway chỉ làm proxy
// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer(); // Có thể không cần
// builder.Services.AddSwaggerGen(); // Swagger cho Gateway có thể phức tạp

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

// Bỏ app.UseHttpsRedirection(); nếu Gateway chạy trên HTTP nội bộ và reverse proxy (nginx, kestrel) xử lý HTTPS bên ngoài.
// Nếu Gateway trực tiếp nhận HTTPS thì giữ lại.
// app.UseHttpsRedirection();

// (Tùy chọn) CORS
if (app.Environment.IsDevelopment() || true) // Luôn bật CORS cho Gateway (điều chỉnh tùy môi trường)
{
    app.UseCors("AllowFlutterApp");
}


// Middleware để log thông tin request (tùy chọn)

// Quan trọng: Authentication phải đứng trước Ocelot
app.UseAuthentication();
//app.UseAuthorization(); // Ocelot tự xử lý authorization dựa trên cấu hình route
//app.UseMiddleware<TokenLoggingMiddleware>(); // Đảm bảo TokenLoggingMiddleware được tạo đúng
app.UseMiddleware<GlobalExceptionHandlerMiddleware>(); // Global Exception Handler cho các lỗi không mong muốn khác

// Middleware để log claims (chỉ cho debug, xóa hoặc comment lại trong production)
//app.Use(async (context, next) =>
//{
//    if (context.User.Identity?.IsAuthenticated == true)
//    {
//        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
//        logger.LogInformation("--- Authenticated User Claims in Gateway (After Auth Middleware) ---");
//        foreach (var claim in context.User.Claims)
//        {
//            logger.LogInformation("Claim Type: {ClaimType}, Value: {ClaimValue}", claim.Type, claim.Value);
//        }
//        logger.LogInformation("------------------------------------------------------------------");
//    }
//    await next.Invoke();
//});
app.Use(async (context, next) =>
{
    await next.Invoke(); // Cho middleware tiếp theo (Ocelot) chạy

    // Sau khi Ocelot chạy, kiểm tra lại status code và các lỗi Ocelot có thể đã set
    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized && !context.Response.HasStarted)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>(); // Hoặc ILogger<YourSpecificClass>
        logger.LogWarning("Customizing 401 Unauthorized response after Ocelot (or Auth middleware).");
        context.Response.ContentType = "application/json";
        var errorResponse = Result<object?>.Unauthorized();
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }));
    }
    else if (context.Response.StatusCode == StatusCodes.Status403Forbidden && !context.Response.HasStarted)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Customizing 403 Forbidden response after Ocelot (or Auth middleware).");
        context.Response.ContentType = "application/json";
        var errorResponse = Result<object?>.Forbidden();
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }));
    }
});



await app.UseOcelot(); // Middleware Ocelot

// app.MapControllers(); // Không cần nếu không có controller trong Gateway

app.Run();