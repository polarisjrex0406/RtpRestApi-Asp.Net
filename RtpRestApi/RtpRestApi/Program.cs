using RtpRestApi.Entities;
using RtpRestApi.Helpers;
using RtpRestApi.Middlewares;
using RtpRestApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.DataProtection;
using Quartz;
using RtpRestApi.QuartzServices;
using Quartz.AspNetCore;

namespace RtpRestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<RtpServerSettings>(builder.Configuration.GetSection("RtpDatabase"));

            builder.Services.AddTransient<IAtlasService, AtlasService>();
            builder.Services.AddSingleton<AdminsService>();
            builder.Services.AddTransient<SettingsService>();
            builder.Services.AddTransient<TopicsService>();
            builder.Services.AddTransient<ArtifactsService>();
            builder.Services.AddTransient<ExperimentsService>();
            builder.Services.AddTransient<TestsService>();
            builder.Services.AddTransient<QueuesService>();
            builder.Services.AddTransient<CachePromptsService>();
            builder.Services.AddTransient<ChatGptService>();
            builder.Services.AddSingleton<AdminPasswordsService>();

            // Register the job and its dependencies with the DI container
            builder.Services.AddScoped<IJob, TestRunJob>();

            // Configure Quartz to use the job
            builder.Services.AddQuartz(q =>
            {
                q.AddJob<TestRunJob>(j => j
                    .WithIdentity("myJob")
                    .DisallowConcurrentExecution()
                    .StoreDurably());

                q.AddTrigger(t => t
                    .ForJob("myJob")
                    .StartNow()
                    .WithSimpleSchedule(s => s
                        .WithIntervalInMinutes(1)
                        .WithMisfireHandlingInstructionFireNow()
                        .RepeatForever()));
            });

            builder.Services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();

            // configure dependency injection
            builder.Services.AddScoped<Services.IAuthenticationService, Services.AuthenticationService>();

            // configure strongly typed settings object
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

            var keysDirectoryName = "Keys";
            var keysDirectoryPath = Path.Combine(builder.Environment.ContentRootPath, keysDirectoryName);
            if (!Directory.Exists(keysDirectoryPath))
            {
                Directory.CreateDirectory(keysDirectoryPath);
            }
            builder.Services.AddDataProtection()
              .PersistKeysToFileSystem(new DirectoryInfo(keysDirectoryPath))
              .SetApplicationName("CustomCookieAuthentication");

            // configure Cookie Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option => {
                    option.Cookie.Domain = builder.Environment.IsDevelopment() ? "localhost" : "ruletheprompt.com";
                    option.ExpireTimeSpan = TimeSpan.FromDays(1);
                    option.Cookie.MaxAge = TimeSpan.FromDays(1);
                    option.Cookie.HttpOnly = true;
                    option.SlidingExpiration = false;
                    option.LoginPath = "/api/login";
                    option.LogoutPath = "/api/logout";
                    option.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = async (context) =>
                        {
                            // validates the cookie
                            var secretKey = builder.Configuration["AppSettings:SecretKey"];
                            await CookieHelper.ValidateCookie(context, secretKey != null ? secretKey : string.Empty);
                        }
                    };
                });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors(builder =>
                builder.WithOrigins("https://ruletheprompt.com", "http://ruletheprompt.com",
                    "https://rtpserver.ruletheprompt.com", "http://rtpserver.ruletheprompt.com",
                    "https://ai-poc-lake.vercel.app", "http://ai-poc-lake.vercel.app",
                    "https://localhost:3000", "http://localhost:3000")
                .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD")
                .AllowAnyHeader()
                .AllowCredentials()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<CookieMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}