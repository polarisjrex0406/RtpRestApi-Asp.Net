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
        public string? artifactName { get; set; }
        public List<Chat>? input { get; set;}
        public Chat? output { get; set; }
    }

    public class PerPromptResponse
    {
        public string? initPrompt { get; set; }
        public List<History>? chatHistory { get; set; }
    }
    public class ExperimentInResponse
    {
        public string? experimentCode { get; set; }
        public string style { get; set; } = "Stand-alone";
        public List<PerPromptResponse>? responses { get; set; }
    }
    public class ExperimentInRequest
    {
        public string? experiment { get; set; }
    }
    public class TestRequest
    {
        public string? testCode { get; set; }
        public string? topic { get; set; }
        public List<ExperimentInRequest>? experiments { get; set; }
    }
    public class TestResponse
    {
        public string? _id { get; set; }
        public bool removed { get; set; } = false;
        public bool enabled { get; set; } = true;
        public string? testCode { get; set; }
        public string? description { get; set; }
        public string? topicName { get; set; }
        public List<ExperimentInResponse>? experiments { get; set; }
        public List<string>? topicPrompt { get; set; }
        public DateTime created { get; set; } = DateTime.Now;
        public DateTime updated { get; set; } = DateTime.Now;
        public bool isPublic { get; set; } = true;
        public string? createdBy { get; set; }
        public string status { get; set; } = "New";
        public string? topic_req { get; set; }
        public List<ExperimentInRequest>? experiments_req { get; set; }
    }
}
