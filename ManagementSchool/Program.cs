using System.Text;
using System.Text.Json.Serialization;
using ManagementSchool.Entities.MomoOptonModel;
using ManagementSchool.Models;
using ManagementSchool.Service;
using ManagementSchool.Service.MomoService;
using ManagementSchool.Service.OrderService;
using ManagementSchool.Service.ParentService;
using ManagementSchool.Service.RefreshToken;
using ManagementSchool.Service.StudentService;
using ManagementSchool.Service.TeacherService;
using ManagementSchool.Service.TuitionFeeNotificationService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using User.ManagementSchool.Service.Models;
using User.ManagementSchool.Service.Service;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Thêm cấu hình HttpClient
builder.Services.AddHttpClient();

// Thêm cấu hình MoMoAPI
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

// --- CONFIGURE DATABASE CONNECTION ---
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (string.IsNullOrEmpty(databaseUrl))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}
else
{
    // Sử dụng NpgsqlConnectionStringBuilder để parse connection string một cách an toàn
    // Thay vì tự parse thủ công dễ gây lỗi với các ký tự đặc biệt
    try 
    {
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');
        var username = userInfo.Length > 0 ? userInfo[0] : string.Empty;
        var password = userInfo.Length > 1 ? userInfo[1] : string.Empty;
        
        var builderDb = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
            Username = username,
            Password = password,
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };
        connectionString = builderDb.ToString();
    }
    catch
    {
        // Fallback: Nếu parse thất bại (do format khác), dùng nguyên chuỗi gốc
        // Npgsql có thể tự xử lý format "postgres://..." trong các phiên bản mới
        connectionString = databaseUrl;
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Cấu hình Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cấu hình xác thực JWT
builder.Services.Configure<IdentityOptions>(opts => opts.SignIn.RequireConfirmedEmail = true);
var jwtSecret = configuration["JWT:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException("JWT:Secret must be configured.");
}
var jwtIssuer = configuration["JWT:validIssuer"];
var jwtAudience = configuration["JWT:validAudience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
        ValidIssuer = jwtIssuer,
        ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

// Cấu hình dịch vụ Email
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
if (emailConfig is null)
{
    throw new InvalidOperationException("EmailConfiguration must be configured.");
}
builder.Services.AddSingleton(emailConfig);

// Đăng ký các dịch vụ
builder.Services.AddScoped<ITuitionFeeNotificationService, TuitionFeeNotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMomoService, MomoService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IParentService, ParentService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IOrderServices, OrderServices>();

var configuredAllowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

// Cấu hình Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Management School API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter your JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Cấu hình Json serializer
builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", corsPolicyBuilder =>
    {
        var effectiveOrigins = configuredAllowedOrigins;
        if (effectiveOrigins.Length == 0)
        {
            if (!builder.Environment.IsDevelopment())
            {
                throw new InvalidOperationException("Cors:AllowedOrigins must be configured outside development.");
            }

            effectiveOrigins = new[] { "http://localhost:3000", "http://localhost:5172", "http://localhost:5173" };
        }

        corsPolicyBuilder
            .WithOrigins(effectiveOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Build app
var app = builder.Build();

// Auto migrate database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Retry mechanism for database connection
        var retryCount = 5;
        var delay = TimeSpan.FromSeconds(3);
        
        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                if (context.Database.CanConnect())
                {
                    context.Database.Migrate();
                    break;
                }
                else 
                {
                    // Attempt to migrate anyway, as CanConnect might fail if DB doesn't exist but server is up
                    context.Database.Migrate();
                    break;
                }
            }
            catch (Exception)
            {
                if (i == retryCount - 1) throw;
                Console.WriteLine($"Database not ready. Retrying in {delay.TotalSeconds} seconds... ({i + 1}/{retryCount})");
                System.Threading.Thread.Sleep(delay);
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Pipeline HTTP request
// if (app.Environment.IsDevelopment()) // Cho phép Swagger chạy cả ở Production
// {
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Management School API V1");
        c.RoutePrefix = "swagger"; // Đảm bảo đường dẫn là /swagger
    });
// }

// Lắng nghe port Railway cung cấp
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

// app.UseHttpsRedirection(); // Tắt HTTPS redirection trong Docker để tránh lỗi 404
app.UseCors("AllowSpecificOrigin");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Tự động chuyển hướng trang chủ về Swagger để dễ dùng
app.MapGet("/", async context =>
{
    context.Response.Redirect("/swagger/index.html");
    await Task.CompletedTask;
});

app.Run();
