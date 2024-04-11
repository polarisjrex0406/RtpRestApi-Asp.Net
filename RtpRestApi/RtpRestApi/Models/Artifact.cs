﻿using System.Text.Json.Serialization;

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
    public class Example
    {
        public string? parserOutputExample { get; set; }
        public string? SampleResponse { get; set; }
    }
    public class ChatGPTSetting {
        public string? setting { get; set; }
        public string? valueType { get; set; }
        public string? value { get; set; }
        public string? minValue { get; set; }
        public string? maxValue { get; set; }
        public string? description { get; set; }
    }
    public class CacheCondition
    {
        public string? key { set; get; }
        public string? changeDetection { set; get; }
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
        public Example? examples { get; set; }
        public List<ChatGPTSetting>? chatgptSettings { get; set; }
        public bool useCache { get; set; } = false;
        public string? cacheTimeoutUnit { get; set; } = null;
        public int? cacheTimeoutValue { get; set; }
        public List<CacheCondition>? cacheConditions { get; set; }
        public string? cacheDescription { get; set; }
        public DateTime created { get; set; } = DateTime.Now;
        public DateTime updated { get; set; } = DateTime.Now;
        public bool isPublic { get; set; } = true;
        public string? createdBy { get; set; }
        public bool enabled { get; set; } = true;
        public bool removed { get; set; } = false;
    }
}
