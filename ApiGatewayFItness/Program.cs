using ApiGatewayFitness.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Configuration;
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // Thêm dòng này
})
.AddJwtBearer("FitnessAppBearer", options =>
{
    options.RequireHttpsMetadata = builder.Environment.IsProduction(); // Nên là true cho production
    options.SaveToken = false; // Gateway không cần lưu token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Xác thực Issuer
        ValidateAudience = true, // Xác thực Audience
        ValidateLifetime = true, // Xác thực token còn hạn hay không
        ValidateIssuerSigningKey = true, // Xác thực chữ ký
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Không cho phép sai lệch thời gian
    };

    // --- Thêm Events để log token ---
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // Token đã được xác thực thành công
            // context.SecurityToken là đối tượng JwtSecurityToken
            // context.Request.Headers["Authorization"] chứa chuỗi "Bearer <token>"
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                // Sử dụng ILogger nếu có, hoặc Console.WriteLine cho debug nhanh
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                logger.LogInformation("API Gateway - Token Validated. Token: {Token}", token);
                // Hoặc: Console.WriteLine($"API Gateway - Token Validated. Token: {token}");

                // Bạn cũng có thể log các claims từ context.Principal.Claims
                // foreach (var claim in context.Principal.Claims)
                // {
                //    logger.LogInformation("Claim Type: {ClaimType}, Value: {ClaimValue}", claim.Type, claim.Value);
                // }
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            // Xảy ra lỗi khi xác thực token
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
            logger.LogError(context.Exception, "API Gateway - Authentication Failed.");
            // Hoặc: Console.WriteLine($"API Gateway - Authentication Failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            // Được gọi khi middleware nhận được token từ request (trước khi validate)
            // context.Token là chuỗi token (nếu có)
            var token = context.Token;
            if (!string.IsNullOrEmpty(token))
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                logger.LogDebug("API Gateway - Message Received with Token (first 20 chars): {TokenStart}", token.Length > 20 ? token.Substring(0, 20) + "..." : token);
                // Hoặc: Console.WriteLine($"API Gateway - Message Received with Token (first 20 chars): {(token.Length > 20 ? token.Substring(0, 20) + "..." : token)}");
            }
            return Task.CompletedTask;
        }
    };
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseMiddleware<TokenLoggingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


await app.UseOcelot(); // Middleware Ocelot

app.MapControllers();

app.Run();
