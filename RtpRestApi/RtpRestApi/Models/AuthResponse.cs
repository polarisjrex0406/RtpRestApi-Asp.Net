
namespace RtpRestApi.Models
{
    public class AuthResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public AuthResponse(Admin user, string token)
        {
            Id = user._id;
            Username = user.surname;
            Email = user.email;
            Token = token;
            Name = user.name;
            Role = user.role;
        }
    }
}
