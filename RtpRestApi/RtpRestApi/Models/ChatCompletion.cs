﻿using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace RtpRestApi.Models
{
    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message? Message { get; set; }

        [JsonPropertyName("logprobs")]
        public object? Logprobs { get; set; }

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    public class ChatCompletionResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("created")]
        public int Created { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }

        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }

        [JsonPropertyName("system_fingerprint")]
        public object? SystemFingerprint { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    public class GptError
    {
        [JsonPropertyName("message")]
        public string? ErrMessage { get; set; }
        [JsonPropertyName("type")]
        public string? ErrType { get; set; }
        [JsonPropertyName("param")]
        public string? ErrParam { get; set; }
        [JsonPropertyName("code")]
        public string? ErrCode { get; set; }
    }
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public GptError? GptError { get; set; }
    }
}