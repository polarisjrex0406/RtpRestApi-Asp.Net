using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RtpRestApi.Models
{
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
        public string role { get; set; } = null!;
        [BsonElement("created")]
        public DateTime created { get; set; }
        [BsonElement("enabled")]
        public bool enabled { get; set; } = false!;
        [BsonElement("removed")]
        public bool removed { get; set; } = false!;
    }
}
