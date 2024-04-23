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

namespace RtpRestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<RtpServerSettings>(builder.Configuration.GetSection("RtpDatabase"));

            builder.Services.AddSingleton<IAtlasService, AtlasService>();
            builder.Services.AddSingleton<AdminsService>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<TopicsService>();
            builder.Services.AddSingleton<ArtifactsService>();
            builder.Services.AddSingleton<ExperimentsService>();
            builder.Services.AddSingleton<TestsService>();
            builder.Services.AddSingleton<QueuesService>();
            builder.Services.AddSingleton<CachePromptsService>();
            builder.Services.AddSingleton<ChatGptService>();
            builder.Services.AddSingleton<AdminPasswordsService>();

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
                    option.Cookie.Domain = "ruletheprompt.com";
                    option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    option.Cookie.MaxAge = TimeSpan.FromMinutes(30);
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

            // Configure the HTTP request pipeline.
            /*            if (app.Environment.IsDevelopment())
                        {*/
            app.UseSwagger();
            app.UseSwaggerUI();
            /*            }*/

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