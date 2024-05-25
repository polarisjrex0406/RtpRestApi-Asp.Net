using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RtpRestApi.Models
{
    public class TemplateInRequest
    {
        public int order { get; set; }
        public string? templateCode { get; set; }
    }

    public class TemplateInResponse
    {
        public int order { get; set; }
        [JsonPropertyName("templateId")]
        public string? templateId { get; set; }
        [JsonPropertyName("templateCode")]
        public ArtifactResponse? templateObj { get; set; }
    }

    public class ExperimentRequest
    {
        public string? experimentCode { get; set; }
        public string? description { get; set; }
        public string? style { get; set; }
        public List<string>? initPrompt { get; set; }
        public string? topic { get; set; }
        public List<TemplateInRequest>? templates { get; set; }
    }

    public class ExperimentResponse
    {
        public string? _id { get; set; }
        public bool removed { get; set; } = false;
        public bool enabled { get; set; } = true;
        public string? experimentCode { get; set; }
        public string? description { get; set; }
        public string? style { get; set; }
        public List<string>? initPrompt {  get; set; }
        [JsonPropertyName("topicId")]
        public string? topicId { get; set; }
        [JsonPropertyName("topic")]
        public TopicResponse? topicObj { get; set; }
        public List<TemplateInResponse>? templates { get; set; }
        public string? ruleLogic {  get; set; }
        public List<Rule>? rules { get; set; }
        public DateTime created {  get; set; } = DateTime.Now;
        public DateTime updated { get; set; } = DateTime.Now;
        public bool isPublic { get; set; } = true;
        public string? createdBy { get; set; }
    }
}