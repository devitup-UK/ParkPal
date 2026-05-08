using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ParkPal.API.Middleware;
using ParkPal.API.Models;
using ParkPal.API.Services;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.API;
using ParkPal.Common.Data;
using ParkPal.Common.Data.Interfaces;
using ParkPal.Common.Services;
using ParkPal.Common.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURATION & APP SETTINGS
// ==========================================
var configurationSection = builder.Configuration.GetSection("Configuration");
builder.Services.Configure<AppSettingsConfiguration>(configurationSection);

var configuration = configurationSection.Get<AppSettingsConfiguration>();

// ==========================================
// 2. DATABASE (Upgraded to PostgreSQL! 🐘)
// ==========================================
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection") ?? throw new InvalidOperationException("Database connection string is missing!");

// ==========================================
// 3. AUTHENTICATION & JWT
// ==========================================
var secret = configuration?.Secret ?? throw new InvalidOperationException("Secret is missing!");
var key = Encoding.ASCII.GetBytes(secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.Events = new JwtBearerEvents
    {

        OnTokenValidated = context =>
        {
            var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
            
            var appUserId = context.Principal?.Identity?.Name;
            
            if (appUserId == null || !tokenService.Verify(appUserId))
            {
                context.Fail("Unauthorized");
            }
            return Task.CompletedTask;
        }
    };
    
    x.RequireHttpsMetadata = false; 
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // Required for the handler to work

// 2. Register Health Checks (including a ping to Postgres!)
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgres_db");

// ==========================================
// 4. DEPENDENCY INJECTION
// ==========================================
builder.Services.AddScoped<IThemeParkService, ThemeParkService>();
builder.Services.AddScoped<IParkRepository>(_ => new ParkRepository(connectionString, configuration.CdnBaseUrl));
builder.Services.AddScoped<IAlertRepository>(_ => new AlertRepository(connectionString));
builder.Services.AddScoped<IDeviceRepository>(_ => new DeviceRepository(connectionString));
builder.Services.AddScoped<ICrowdSourceRepository>(_ => new CrowdSourceRepository(connectionString));
builder.Services.AddScoped<IUsersRepository, UsersRepository>(_ => new UsersRepository(connectionString));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAttractionHistoryRepository>(_ => new AttractionHistoryRepository(connectionString));
builder.Services.AddScoped<IItineraryRepository>(_ => new ItineraryRepository(connectionString));
builder.Services.AddScoped<ILiveActivityRepository>(_ => new LiveActivityRepository(connectionString));
builder.Services.AddScoped<IPlanningService, PlanningService>();

// Registers the HTTP Client, sets the base URL for themeparks.wiki, and wires up the ThemeParkApi!
builder.Services.AddHttpClient<ThemeParkApi>(client =>
{
    client.BaseAddress = new Uri(configuration.ThemeParkApiBaseUrl);
})
.AddStandardResilienceHandler();

// ==========================================
// 5. MVC, CORS & SWAGGER
// ==========================================
builder.Services.AddCors();
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==========================================
// 6. LOGGING
// ==========================================
var loggingConnectionString = builder.Configuration.GetConnectionString("LoggingConnection") ?? throw new InvalidOperationException("Logging connection string is missing!");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.Seq(loggingConnectionString) // Points to the Docker container
    .Enrich.WithProperty("Application", "ParkPal.API")
    .CreateLogger();
builder.Host.UseSerilog();

// ==========================================
// 🚀 BUILD THE APP
// ==========================================
var app = builder.Build();

// ==========================================
// 8. HTTP REQUEST PIPELINE
// ==========================================
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => 
{  
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ParkPal.API");  
});  

app.MapControllers();

app.Run();