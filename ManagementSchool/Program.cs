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

// Thêm cấu hình HttpClient
builder.Services.AddHttpClient();

// Thêm cấu hình MoMoAPI
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

// --- CONFIGURE DATABASE CONNECTION ---
string connectionString;
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Railway environment
    var hostPort = databaseUrl.Split(':');
    var dbUser = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
    var dbPass = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
    var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME");

    var builderDb = new NpgsqlConnectionStringBuilder
    {
        Host = hostPort[0],
        Port = int.Parse(hostPort[1]),
        Username = dbUser,
        Password = dbPass,
        Database = dbName,
        SslMode = SslMode.Prefer,
        TrustServerCertificate = true
    };

    connectionString = builderDb.ConnectionString;
}
else
{
    // Local environment
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Cấu hình Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cấu hình xác thực JWT
builder.Services.Configure<IdentityOptions>(opts => opts.SignIn.RequireConfirmedEmail = true);

var configuration = builder.Configuration;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

// Cấu hình dịch vụ Email
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
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
        corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Build app
var app = builder.Build();

// Auto migrate database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Pipeline HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Lắng nghe port Railway cung cấp
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
