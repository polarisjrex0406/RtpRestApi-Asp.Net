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
            builder.Services.Configure<RtpDatabaseSettings>(builder.Configuration.GetSection("RtpDatabase"));

            builder.Services.AddSingleton<IAtlasService, AtlasService>();
            builder.Services.AddSingleton<AdminsService>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<IExperimentService, ExperimentService>();
            builder.Services.AddSingleton<TopicsService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // lets add cors
            builder.Services.AddCors();

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

            // global cors policy
            app.UseCors(options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowCredentials().AllowAnyHeader());

            /*app.UseHttpsRedirection();*/

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<CookieMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}