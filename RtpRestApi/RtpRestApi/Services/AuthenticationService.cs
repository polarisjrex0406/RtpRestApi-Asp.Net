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
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IOptions<AppSettings> appSettings, IUserRepository userRepository)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
        }

        public AuthResponse? Authenticate(AuthRequest requestModel)
        {
            // In production, this will be retrieving user data in a database
            User? user = _userRepository.GetAllUsers().SingleOrDefault(u => u.Username == requestModel.Username && u.Password == requestModel.Password);

            // return null if user is not found
            if (user == null) return null;

            string token = JWTHelper.GenerateJsonWebToken(user, _appSettings);

            return new AuthResponse(user, token);
        }
    }
}
