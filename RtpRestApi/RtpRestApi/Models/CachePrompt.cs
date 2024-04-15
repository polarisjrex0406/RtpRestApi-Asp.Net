using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RtpRestApi.Models
{
    public class CachePromptRequest
    {
        public string? template { get; set; }
        public string? cacheTimeoutUnit { get; set; } = null;
        public int? cacheTimeoutValue { get; set; }
        public List<CacheCondition>? cacheConditions { get; set; }
        public string? initPrompt { get; set; }
        public string? input { get; set; }
        public string? output { get; set; }
    }
    public class CachePromptResponse
    {
        public string? _id {  get; set; }
        public bool removed { get; set; } = false;
        public bool enabled { get; set; } = true;
        public string? template { get; set;}
        public List<CacheCondition>? cacheConditions { get; set;}
        public string? initPrompt { get; set;}
        public string? input {  get; set; }
        public string? output { get; set; }
        public DateTime created { get; set; } = DateTime.Now;
        public DateTime updated { get; set; } = DateTime.Now;
        public DateTime expired {  get; set; }
        public bool isPublic { get; set; } = true;
        public string? createdBy { get; set; }
    }
}
