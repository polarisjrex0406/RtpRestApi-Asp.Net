using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RtpRestApi.Models;

namespace RtpRestApi.Services
{
    public class QueuesService
    {
        private bool CheckOneCondition(string compareItem, string conditionOperator, string conditionValue)
        {
            bool conditionPassed = false, doublePassed = false;
            double tempCompareItem = 0.0, tempConditionValue = 0.0;
            // Try pass to numbers (currently, double)
            if (!compareItem.IsNullOrEmpty() && double.TryParse(compareItem, out tempCompareItem) &&
                !conditionValue.IsNullOrEmpty() && double.TryParse(conditionValue, out tempConditionValue))
            {
                doublePassed = true;
            }

            if (conditionOperator == "EQ")
            {
                if (doublePassed) conditionPassed = (tempCompareItem == tempConditionValue);
                else conditionPassed = (compareItem == conditionValue);
            }
            if (conditionOperator == "NEQ")
            {
                if (doublePassed) conditionPassed = (tempCompareItem != tempConditionValue);
                else conditionPassed = (compareItem != conditionValue);
            }
            if (conditionOperator == "LT")
            {
                if (doublePassed) conditionPassed = (tempCompareItem < tempConditionValue);
                else conditionPassed = false;
            }
            if (conditionOperator == "LTE")
            {
                if (doublePassed) conditionPassed = (tempCompareItem <= tempConditionValue);
                else conditionPassed = false;
            }
            if (conditionOperator == "GT")
            {
                if (doublePassed) conditionPassed = (tempCompareItem > tempConditionValue);
                else conditionPassed = false;
            }
            if (conditionOperator == "GTE" && string.Compare(compareItem, conditionValue) >= 0)
            {
                if (doublePassed) conditionPassed = (tempCompareItem >= tempConditionValue);
                else conditionPassed = false;
            }
            if (conditionOperator == "IN" && compareItem.IndexOf(conditionValue) > -1)
            {
                conditionPassed = true;
            }
            if (conditionOperator == "NOTIN" && compareItem.IndexOf(conditionValue) == -1)
            {
                conditionPassed = true;
            }
            return conditionPassed;
        }
        private bool CheckOneRule(Rule rule, string initPrompt, List<PromptEnhancer>? promptEnhancers, List<History> prevChats)
        {
            bool rulePassed = (rule == null || rule.conditionsLogic == null || rule.conditionsLogic == "All" || rule.conditions == null);
            if (rule != null && rule.conditions != null)
            {
                foreach (var condition in rule.conditions)
                {
                    bool conditionPassed = false;
                    string? compareItem, conditionOperator, conditionValue;
                    compareItem = null;
                    conditionOperator = condition.conditionOperator;
                    conditionValue = condition.conditionValue;
                    if (rule == null) continue;

                    if (condition?.conditionType?.ToLower() == "topicprompt")
                    {
                        compareItem = initPrompt;
                    }
                    else if (condition?.conditionType?.ToLower() == "lastresponse")
                    {
                        if (prevChats != null && prevChats.Count() > 0)
                        {
                            History hist = prevChats.Last();
                            compareItem = hist?.output?.content;
                        }
                    }
                    else if (condition?.conditionType?.ToLower() == "key")
                    {
                        if (promptEnhancers == null) continue;
                        foreach (var enhancer in promptEnhancers)
                        {
                            if (enhancer == null) continue;
                            if (enhancer.key == null) continue;
                            if (condition.conditionItem == null) continue;
                            if (enhancer.key.ToLower() == condition.conditionItem.ToLower())
                            {
                                if (enhancer.value == null) continue;
                                compareItem = enhancer.value;
                            }
                        }
                    }

                    if (compareItem != null && conditionOperator != null && conditionValue != null)
                    {
                        conditionPassed = CheckOneCondition(compareItem, conditionOperator, conditionValue);
                    }
                    else
                    {
                        conditionPassed = true;
                    }

                    rulePassed = (rule.conditionsLogic == "All") ? rulePassed && conditionPassed : rulePassed || conditionPassed;
                }
            }
            return rulePassed;
        }
        public bool CheckCriterias(string? ruleLogic, List<Rule>? rules, string initPrompt, List<PromptEnhancer>? promptEnhancers, List<History> prevChats)
        {
            bool notSkip = (ruleLogic == null || ruleLogic == "All" || rules == null);
            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    bool rulePassed;
                    if (rule == null) continue;
                    rulePassed = CheckOneRule(rule, initPrompt, promptEnhancers, prevChats);
                    notSkip = (ruleLogic == "All") ? notSkip && rulePassed : notSkip || rulePassed;
                }

                if (rules.Count == 0) notSkip = true;
            }
            return notSkip;
        }
        public bool SkipArtifactOrGoChat(string? style, ref List<Chat> messages, ref List<History> prevChats)
        {
            bool skipTemplate = false;
            if (style == "Conversation")
            {
                // conversational style
                if (prevChats != null && prevChats.Count() > 0)
                {
                    History hist = prevChats.Last();
                    if (hist != null && hist.input != null && hist.output != null)
                    {
                        foreach (Chat input in hist.input)
                        {
                            messages.Add(input);
                        }
                        messages.Add(hist.output);
                    }
                }
            }
            return skipTemplate;
        }
        public string ApplyPromptEnhancers(List<PromptEnhancer>? promptEnhancers, string? promptOutput, string init_prompt, ref List<Chat> messages)
        {
            string enhancedPrompt = (promptOutput == null) ? "" : promptOutput;
            if (promptEnhancers != null)
            {
                foreach (var enhancer in promptEnhancers)
                {
                    string pattern = "{{" + enhancer.key + "}}";
                    enhancedPrompt = enhancedPrompt.Replace(pattern, enhancer.value);
                }
            }
            enhancedPrompt = enhancedPrompt.Replace("{{TopicPrompt}}", init_prompt);

            Chat msg = new Chat();
            msg.role = "user";
            msg.content = enhancedPrompt;
            messages.Add(msg);

            return enhancedPrompt;
        }
        public JObject ConfigGptSettings(List<ChatGPTSetting>? settingsFromDb)
        {
            JObject gptSettings = new JObject();
            if (settingsFromDb == null)
            {
                gptSettings["model"] = "gpt-3.5";
            }
            else
            {
                foreach (var settingObj in settingsFromDb)
                {
                    if (settingObj == null) continue;
                    if (settingObj.setting == null || settingObj.value == null) continue;

                    string key;
                    if (settingObj.setting == "language_model")
                    {
                        key = "model";
                    }
                    else if (settingObj.setting == "stop_sequences")
                    {
                        key = "stop";
                    }
                    else key = settingObj.setting;

                    JObject gptSetting = new JObject();
                    if (settingObj.valueType == "integer")
                    {
                        gptSettings[key] = int.Parse(settingObj.value);
                    }
                    else if (settingObj.valueType == "float")
                    {
                        gptSettings[key] = float.Parse(settingObj.value);
                    }
                    else gptSettings[key] = settingObj.value;
                }
            }
            return gptSettings;
        }
    }
}
