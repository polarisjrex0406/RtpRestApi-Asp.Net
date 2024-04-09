using RtpRestApi.Entities;
using RtpRestApi.Helpers;
using RtpRestApi.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace RtpRestApi.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppSettings _appSettings;
        private readonly AdminsService _adminsService;

        public AuthenticationService(IOptions<AppSettings> appSettings, AdminsService adminsService)
        {
            _appSettings = appSettings.Value;
            _adminsService = adminsService;
        }

        public AuthResponse? Authenticate(AuthRequest requestModel)
        {           
            Admin? user = null;
            if (user == null)
            {
                return null;
            }

            string token = JWTHelper.GenerateJsonWebToken(user, _appSettings);

            return new AuthResponse(user, token);
        }
    }
}
