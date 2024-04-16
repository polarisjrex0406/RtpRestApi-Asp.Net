using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RtpRestApi.Models
{
    [BsonIgnoreExtraElements]
    public class AdminPassword
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; } = null!;

        [BsonElement("user")]
        public string user { get; set; } = null!;

        [BsonElement("password")]
        public string password { get; set; } = null!;

        [BsonElement("salt")]
        public string salt { get; set; } = null!;

        [BsonElement("emailToken")]
        public string emailToken { get; set; } = null!;

        [BsonElement("resetToken")]
        public string resetToken { get; set; } = null!;

        [BsonElement("emailVerified")]
        public bool emailVerified { get; set; } = false!;

        [BsonElement("authType")]
        public string authType { get; set; } = "email"!;

        [BsonElement("loggedSessions")]
        public List<string>? loggedSessions { get; set; }

        [BsonElement("enabled")]
        public bool enabled { get; set; } = false!;

        [BsonElement("removed")]
        public bool removed { get; set; } = false!;

    }
}
