using System.ComponentModel.DataAnnotations;

namespace RtpRestApi.Models
{
    public class TemplateInExperiment
    {
        public int order { get; set; }
        public string? templateCode { get; set; }
    }

    public class RuleInExperiment
    {
        public string? ruleName { get; set; }
        public string? conditionLogic { get; set; }
        public string? conditionName { get; set; }
        public string? conditionType { get; set; }
        public string? conditionItem { get; set; }
        public string? conditionOperator { get; set; }
        public string? conditionValue { get; set; }
    }

    public class ExperimentRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool Remember { get; set; } = true;
    }

    public class ExperimentResponse
    {
        public string? _id { get; set; }
        public bool removed { get; set; }
        public bool enabled { get; set; }
        public string? experimentCode { get; set; }
        public string? description { get; set; }
        public string? style { get; set; }
        public string? initPrompt {  get; set; }
        public string? topic { get; set; }
        public List<TemplateInExperiment>? templates { get; set; }
        public string? ruleLogic {  get; set; }
        public List<RuleInExperiment>? rules { get; set; }
        public DateTime created {  get; set; }
        public DateTime updated { get; set; }
        public bool isPublic { get; set; }
        public string? createdBy { get; set; }
    }
}