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
// ?? תוספת חובה: רישום הגשר כ-Singleton כדי לשמור על האירועים בזיכרון
builder.Services.AddSingleton<IMailNotificationBridge, SimpleNotificationBridge>();

// ==========================================
// BL Services
// ==========================================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EventsService>();
builder.Services.AddScoped<EventManagerService>();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<PollService>();
// ?? תוספת: רישום שירות ההתראות מה-BL שלכן
builder.Services.AddScoped<NotificationService>();

// ==========================================
// Controllers & Swagger
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==========================================
// JWT Authentication
// ==========================================
string jwtKey = builder.Configuration["Jwt:Key"] ?? "default-secret-key-replace-in-production";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();