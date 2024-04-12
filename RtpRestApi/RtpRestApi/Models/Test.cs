using System.Text.Json.Serialization;

namespace RtpRestApi.Models
{
    public class Chat
    {
        public string? role { get; set; }
        public string? content { get; set; }
    }
    public class History
    {
        public List<Chat>? input { get; set;}
        public Chat? output { get; set; }
    }
    public class ExperimentInResponse
    {
        public int order { get; set; }
        [JsonPropertyName("experimentNo")]
        public string? experimentId { get; set; }
        [JsonPropertyName("experiment")]
        public ExperimentResponse? experimentObj { get; set; }
        public List<History>? chatHistory { get; set; }
    }
    public class TestRequest
    {
        public string? testCode { get; set; }
        public string? topic { get; set; }
        public List<ExperimentRequest>? experiments { get; set; }
    }
    public class TestResponse
    {
        public string? _id { get; set; }
        public bool removed { get; set; } = false;
        public bool enabled { get; set; } = true;
        public string? testCode { get; set; }
        public string? description { get; set; }
        [JsonPropertyName("topicId")]
        public string? topicId { get; set; }
        [JsonPropertyName("topic")]
        public TopicResponse? topicObj { get; set; }
        public List<ExperimentInResponse>? experiments { get; set; }
        public DateTime created { get; set; } = DateTime.Now;
        public DateTime updated { get; set; } = DateTime.Now;
        public bool isPublic { get; set; } = true;
        public string? createdBy { get; set; }
    }
}
