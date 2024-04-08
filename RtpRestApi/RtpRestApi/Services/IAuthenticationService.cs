using RtpRestApi.Models;

namespace RtpRestApi.Services
{
    public interface IAuthenticationService
    {
        AuthResponse? Authenticate(AuthRequest requestModel);
    }
}
