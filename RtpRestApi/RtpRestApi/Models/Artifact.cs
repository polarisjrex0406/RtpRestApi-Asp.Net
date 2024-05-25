using System.Text.Json.Serialization;

namespace RtpRestApi.Models
{
    public class PromptEnhancer
    {
        public string? key { get; set; }
        public string valueType { get; set; } = "text";
        public string? value { get; set; }
        public string? description { get; set; }
        public string? promptModifier { get; set; }
    }
    public class ChatGPTSetting {
        public string? setting { get; set; }
        public string? valueType { get; set; }
        public string? value { get; set; }
        public string? minValue { get; set; }
        public string? maxValue { get; set; }
        public string? description { get; set; }
    }
    public class Condition
    {
        public string? conditionName { get; set; }
        public string? conditionType { get; set; }
        public string? conditionItem { get; set; }
        public string? conditionOperator { get; set; }
        public string? conditionValue { get; set; }
    }
    public class Rule
    {
        public string? ruleName { get; set; }
        public string? conditionsLogic { get; set; }
        public List<Condition>? conditions { get; set; }
    }
    public class CacheCondition
    {
        public string? key { set; get; }
        public string? changeDetection { set; get; }
    }

    public class ArtifactRequest
    {
        public string? name { get; set; }
        public string? group { get; set; } = null;
        public string? goal { get; set; } = null;
        public string? topic { get; set; }
        public List<PromptEnhancer>? promptEnhancers { get; set; } = null;
        public string? promptOutput { get; set; } = null;
        public List<ChatGPTSetting>? chatgptSettings { get; set; } = null;
        public string? ruleLogic { get; set; }
        public List<Rule>? rules { get; set; }
        public bool useCache { get; set; } = false;
        public string? cacheTimeoutUnit { get; set; } = null;
        public int cacheTimeoutValue { get; set; }
        public List<CacheCondition>? cacheConditions { get; set; }
    }

    public class ArtifactResponse
    {
        public string? _id { get; set; }
        public string? name { get; set; }
        public string? group { get; set; }
        public string? goal { get; set; }
        [JsonPropertyName("topicId")]
        public string? topicId { get; set; }
        [JsonPropertyName("topic")]
        public TopicResponse? topicObj { get; set; }
        public List<PromptEnhancer>? promptEnhancers { get; set; }
        public string? promptOutput { get; set; }
        public List<ChatGPTSetting>? chatgptSettings { get; set; }
        public string? ruleLogic { get; set; }
        public List<Rule>? rules { get; set; }
        public bool useCache { get; set; } = false;
        public string? cacheTimeoutUnit { get; set; } = null;
        public int? cacheTimeoutValue { get; set; }
        public List<CacheCondition>? cacheConditions { get; set; }
        public DateTime created { get; set; } = DateTime.Now;
        public DateTime updated { get; set; } = DateTime.Now;
        public bool isPublic { get; set; } = true;
        public string? createdBy { get; set; }
        public bool enabled { get; set; } = true;
        public bool removed { get; set; } = false;
    }
}
