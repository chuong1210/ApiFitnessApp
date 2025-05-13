using Infrastructure;
using Application.Common.Interfaces;
using FitnessApp.Services;
using Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FitnessApp.Middleware;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Ocelot.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Google;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var redisConfig = builder.Configuration.GetSection("Redis");
var configuration = builder.Configuration;
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

// Validate JWT settings
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 16) // Key cần đủ dài và phức tạp
{
    throw new ArgumentNullException("JWT Key is missing or too short in configuration.");
}
if (string.IsNullOrEmpty(jwtIssuer)) { throw new ArgumentNullException("JWT Issuer is missing in configuration."); }
if (string.IsNullOrEmpty(jwtAudience)) { throw new ArgumentNullException("JWT Audience is missing in configuration."); }
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // --- Cấu hình Swagger để hỗ trợ JWT ---
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // Thêm dòng này
})
.AddJwtBearer(options =>
{
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
})
    // options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Nếu dùng cookie
    // options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Chỉ khi dùng server-side flow
    // .AddCookie() // Nếu dùng cookie
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options => // Đăng ký handler Google
    {
        // Lấy thông tin từ cấu hình
        IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"]!;
        options.ClientSecret = googleAuthNSection["ClientSecret"]!;
        // options.CallbackPath = "/signin-google"; // Callback mặc định nếu dùng server-side flow
    });

// --- Cấu hình Authorization (nếu cần policy) ---
//builder.Services.AddStackExchangeRedisCach(options =>
//{
//    options.Configuration = $"{redisConfig["Host"]}:{redisConfig["Port"]}";
//    if (!string.IsNullOrEmpty(redisConfig["Password"]))
//    {
//        options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
//        {
//            EndPoints = { $"{redisConfig["Host"]}:{redisConfig["Port"]}" },
//            Password = redisConfig["Password"]
//        };
//    }
//});

// --- Cấu hình Hangfire ---
// 1. Thêm Hangfire Services và cấu hình Storage (SQL Server)
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180) // Đặt mức tương thích
    .UseSimpleAssemblyNameTypeSerializer()
.UseRecommendedSerializerSettings()
    .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions // Dùng connection string của DB chính hoặc DB riêng cho Hangfire
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero, // Giảm độ trễ polling
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true // Khuyến nghị cho SQL Server để tránh deadlock
    }));
// 2. Thêm Hangfire Server vào services (để chạy background jobs)
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 2; // Số worker xử lý job (tùy chỉnh)
    // options.Queues = new[] { "default", "emails", "reports" }; // Định nghĩa các queue nếu cần
});

builder.Services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPermissionPoliciesFromAttributes(Assembly.GetExecutingAssembly());
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://yourappdomain.com") // Frontend URLs
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard("/hangfire", new DashboardOptions // Endpoint truy cập dashboard
{
    // --- CẤU HÌNH AUTHORIZATION CHO DASHBOARD ---
    // Cách 1: Đơn giản (chỉ cho phép local access - KHÔNG AN TOÀN cho production)
    // Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() }

    // Cách 2: Custom (Yêu cầu đăng nhập và có quyền cụ thể)
    Authorization = new[] { new HangfireDashboardAuthorizationFilter() } // Xem implementation bên dưới
    // ------------------------------------------------
});
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
async Task InitializePermissions(IServiceProvider serviceProvider)
{
    var permissionService = serviceProvider.GetRequiredService<IPermissionService>();

    List<string> permissions = AuthorizationExtensions
            .GetPermissionPoliciesFromAttributes(Assembly.GetExecutingAssembly());
    await permissionService.Create(permissions);
}

//dotnet ef migrations add InitialCreateSqlServerIntGender  --context AppDbContext --project Infrastructure --startup-project FitnessApp --output-dir Migrations

//dotnet ef database update --project Infrastructure --startup-project FitnessApp