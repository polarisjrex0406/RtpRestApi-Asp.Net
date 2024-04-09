using System.ComponentModel.DataAnnotations;

namespace RtpRestApi.Models
{
    public class AuthRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool Remember { get; set; } = true;
    }
}
