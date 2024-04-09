using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RtpRestApi.Models
{
    [BsonIgnoreExtraElements]
    public class Setting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; } = null!;

        [BsonElement("settingCategory")]
        public string settingCategory { get; set; } = null!;

        [BsonElement("settingKey")]
        public string settingKey { get; set; } = null!;

        [BsonElement("settingValue")]
        public object settingValue { get; set; } = null!;

        [BsonElement("valueType")]
        public string valueType { get; set; } = "String";

        [BsonElement("isPrivate")]
        public bool isPrivate { get; set; } = false;

        [BsonElement("isCoreSetting")]
        public bool isCoreSetting { get; set; } = false;

        [BsonElement("enabled")]
        public bool enabled { get; set; } = false;

        [BsonElement("removed")]
        public bool removed { get; set; } = false;
    }
}
