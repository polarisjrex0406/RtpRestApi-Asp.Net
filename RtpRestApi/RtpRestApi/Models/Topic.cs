using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;

namespace RtpRestApi.Models
{
    public class TopicRequest
    {
        [Required]
        public string name { get; set; } = string.Empty;
        public string group { get; set; } = string.Empty;
        public string goal { get; set; } = string.Empty;
        public string topicPrompt { get; set; } = string.Empty;
    }
    public class TopicResponse
    {
        public string? _id { get; set; }
        public string? name { get; set; }
        public string? group { get; set; }
        public string? goal { get; set; }
        public string? topicPrompt { get; set; }
        public DateTime created { get; set; } = DateTime.Now;
        public DateTime updated { get; set; } = DateTime.Now;
        public bool isPublic { get; set; } = true;
        public string? createdBy { get; set; }
        public bool enabled { get; set; } = true;
        public bool removed { get; set; } = false;
    }
}
