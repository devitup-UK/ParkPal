using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using ParkPal.API.Models;
using ParkPal.API.Services;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.Database.Contexts;
using ParkPal.Common.Logging.Providers;
using ParkPal.Common.Models.Configuration;
using ParkPal.Common.Services;
using ParkPal.Common.Services.Interfaces;

namespace ParkPal.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>();

            services.AddCors();
            services.AddControllers();

            // configure strongly typed settings objects
            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            AppSettings appSettings = appSettingsSection.Get<AppSettings>();
            ConfigurationService configurationService = new(appSettings);
            configurationService.ConfigureSettings();
            
            var key = Encoding.ASCII.GetBytes(Settings.Secret);


            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string authorization = context.Request.Headers["x-token"];

                        // If no authorization header found, nothing to process further
                        if (string.IsNullOrEmpty(authorization))
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }

                        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authorization.Substring("Bearer ".Length).Trim();
                        }

                        // If no token found, no further work possible
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        ITokenService tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                        string token = context.Principal.Identity.Name;
                        bool tokenValid = tokenService.Verify(token);
                        if (!tokenValid)
                        {
                            // return unauthorized if user no longer exists
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

            // configure DI for application services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IThemeParkService, ThemeParkService>();
            
            services.AddMvc().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            services.AddLogging();

            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext dataContext, ILoggerFactory loggerFactory)
        {
            AppSettings appSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();

            // Check database version and see if it is the latest, if it is not, then upgrade the database to the latest version with our upgrade scripts.
            DatabaseUpgradeService upgradeService = new DatabaseUpgradeService(Settings.SQLConnectionString, "Database/Scripts");
            upgradeService.UpgradeDatabase();
            
            loggerFactory.AddProvider(new DbLoggerProvider(_configuration));

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseSwagger();  
            app.UseSwaggerUI(c => {  
                c.SwaggerEndpoint("v1/swagger.json", "ParkPal.API");  
            });  

        }
    }
}
