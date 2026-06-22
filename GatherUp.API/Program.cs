using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GatherUp.Core.Interfaces;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Users;
using GatherUp.Core.DO.Finance;
using GatherUp.Core.DO.Polls;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.Mail;
using GatherUp.BL.Services;
using GatherUp.API.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ==========================================
// Repositories
// ==========================================
builder.Services.AddScoped<IRepository<Event>, XmlRepository<Event>>();
builder.Services.AddScoped<IRepository<Participant>, XmlRepository<Participant>>();
builder.Services.AddScoped<IRepository<VendorAllocation>, XmlRepository<VendorAllocation>>();
builder.Services.AddScoped<IRepository<ReceiptDetails>, ReceiptRepository>();
builder.Services.AddScoped<IRepository<Poll>, XmlRepository<Poll>>();
builder.Services.AddScoped<IRepository<Person>, XmlRepository<Person>>();


// ==========================================
// Mail Service & Notification Bridge
// ==========================================
builder.Services.AddScoped<IMailService, FileMailService>();
// 🌟 תוספת חובה: רישום הגשר כ-Singleton כדי לשמור על האירועים בזיכרון
builder.Services.AddSingleton<IMailNotificationBridge, SimpleNotificationBridge>();

// ==========================================
// BL Services
// ==========================================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EventsService>();
builder.Services.AddScoped<EventManagerService>();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<PollService>();
// 🌟 תוספת: רישום שירות ההתראות מה-BL שלכן
builder.Services.AddScoped<NotificationService>();

// ==========================================
// 🌟 תוספת: הגדרת פוליסי עבור CORS 🌟
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // מאפשר לכל קובץ HTML מקומי לפנות ל-API
              .AllowAnyMethod()   // מאפשר את כל הפעולות (GET, POST, PUT וכו')
              .AllowAnyHeader();  // מאפשר להעביר כותרות אבטחה (כמו ה-Bearer Token)
    });
});

// ==========================================
// Controllers & Swagger
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "GatherUp.API", Version = "v1" });

    // הגדרת תצורת האבטחה של ה-Bearer Token עבור Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
 Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// ==========================================
// JWT Authentication
// ==========================================
string jwtKey = builder.Configuration["Jwt:Key"] ?? "default-secret-key-replace-in-production";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ==========================================
// Build
// ==========================================
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ==========================================
// Middleware Pipeline (strict order)
// ==========================================
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseStaticFiles();
app.UseRouting();

// 🌟 תוספת: הפעלת ה-CORS בצינור העבודה (ממש לפני ה-Auth) 🌟
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();