using RtpRestApi.Entities;
using RtpRestApi.Helpers;
using RtpRestApi.Middlewares;
using RtpRestApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;

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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // lets add cors
/*            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("https://ruletheprompt.com", "http://ruletheprompt.com",
                            "https://ai-poc-lake.vercel.app", "http://ai-poc-lake.vercel.app")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowedToAllowWildcardSubdomains();
                    });
            });*/

            builder.Services.AddHttpClient();

            // configure dependency injection
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<Services.IAuthenticationService, Services.AuthenticationService>();

            // configure strongly typed settings object
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


            // configure Cookie Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option => option.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = async (context) =>
                    {
                        // validates the cookie
                        await CookieHelper.ValidateCookie(context, builder.Configuration["AppSettings:SecretKey"]);
                    }
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // global cors policy
            /*app.UseCors(options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowCredentials().AllowAnyHeader());*/
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors(builder =>
                builder.WithOrigins("https://ruletheprompt.com", "http://ruletheprompt.com",
                    "https://ai-poc-lake.vercel.app", "http://ai-poc-lake.vercel.app")
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<CookieMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}