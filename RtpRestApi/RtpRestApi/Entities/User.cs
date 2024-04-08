using System.Text.Json.Serialization;

namespace RtpRestApi.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FavoriteColor { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"*** User ***\nId: {Id}\nName: {Name}\nEmail: {Email}\nFavorite Color: {FavoriteColor}\nUsername: {Username}\n*** User ***";
        }
    }
}
