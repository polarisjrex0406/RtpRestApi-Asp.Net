using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RtpRestApi.Models
{
    public class AdminRequest
    {
        [BsonElement("name")]
        public string name { get; set; } = null!;

        [BsonElement("country")]
        public string? country { get; set; } = null!;

        [BsonElement("email")]
        public string email { get; set; } = null!;

        [BsonElement("password")]
        public string? password { get; set; } = null!;

        [BsonElement("surname")]
        public string? surname { get; set; } = null!;

        [BsonElement("enabled")]
        public bool? enabled { get; set; }

        [BsonElement("role")]
        public string? role { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Admin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; } = null!;
        [BsonElement("name")]
        public string name { get; set; } = null!;
        [BsonElement("surname")]
        public string surname { get; set; } = null!;
        [BsonElement("email")]
        public string email { get; set; } = null!;
        [BsonElement("photo")]
        public string photo { get; set; } = null!;
        [BsonElement("role")]
        public string role { get; set; } = "employee";
        [BsonElement("created")]
        public DateTime created { get; set; } = DateTime.Now;
        [BsonElement("enabled")]
        public bool enabled { get; set; } = false!;
        [BsonElement("removed")]
        public bool removed { get; set; } = false!;
    }
}
