using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Newtonsoft.Json.Linq;
using RtpRestApi.Models;

namespace RtpRestApi.Services
{
    public class QueuesService
    {
        private bool CheckOneCriteria(string compareItem, string conditionOperator, string conditionValue)
        {
            bool criteriaPassed = false;
            if (conditionOperator == "EQ" && compareItem == conditionValue)
            {
                criteriaPassed = true;
            }
            if (conditionOperator == "NEQ" && compareItem != conditionValue)
            {
                criteriaPassed = true;
            }
            if (conditionOperator == "LT" && string.Compare(compareItem, conditionValue) < 0)
            {
                criteriaPassed = true;
            }
            if (conditionOperator == "LTE" && string.Compare(compareItem, conditionValue) <= 0)
            {
                criteriaPassed = true;
            }
            if (conditionOperator == "GT" && string.Compare(compareItem, conditionValue) > 0)
            {
                criteriaPassed = true;
            }
            if (conditionOperator == "GTE" && string.Compare(compareItem, conditionValue) >= 0)
            {
                criteriaPassed = true;
            }
            if (conditionOperator == "IN" && compareItem.IndexOf(conditionValue) > -1)
            {
                criteriaPassed = true;
            }
            if (conditionOperator == "NOTIN" && compareItem.IndexOf(conditionValue) == -1)
            {
                criteriaPassed = true;
            }
            return criteriaPassed;
        }
        public bool CheckCriterias(string? ruleLogic, List<Rule>? rules, string? initPrompt, List<PromptEnhancer>? promptEnhancers)
        {
            bool notSkip = (ruleLogic == null || ruleLogic == "All" || rules == null);
            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    bool criteriaPassed;
                    string? compareItem, conditionOperator, conditionValue;
                    compareItem = null;
                    conditionOperator = rule.conditionOperator;
                    conditionValue = rule.conditionValue;
                    if (rule == null) continue;

                    if (rule.conditionType == "initPrompt" || rule.conditionType == "initial_prompt" || rule.conditionType == "init_prompt")
                    {
                        compareItem = initPrompt;
                    }
                    else if (rule.conditionType == "key" || rule.conditionType == "Key")
                    {
                        if (promptEnhancers == null) continue;
                        foreach (var enhancer in promptEnhancers)
                        {
                            if (enhancer == null) continue;
                            if (enhancer.key == null) continue;
                            if (rule.conditionItem == null) continue;
                            if (enhancer.key.ToLower() == rule.conditionItem.ToLower())
                            {
                                if (enhancer.value == null) continue;
                                compareItem = enhancer.value;
                            }
                        }
                    }

                    if (compareItem != null && conditionOperator != null && conditionValue != null)
                    {
                        criteriaPassed = CheckOneCriteria(compareItem, conditionOperator, conditionValue);
                    }
                    else
                    {
                        criteriaPassed = true;
                    }
                    notSkip = (ruleLogic == "All") ? notSkip && criteriaPassed : notSkip || criteriaPassed;
                }

                if (rules.Count == 0) notSkip = true;
            }
            return notSkip;
        }
        public bool SkipArtifactOrGoChat(string style, string? initPrompt, ref List<Chat> messages, ref List<History> prevChats)
        {
            bool skipTemplate = false;
            if (style == "Stand-alone")
            {
                // initial prompt as a system message
                Chat msg = new Chat();
                msg.role = "system";
                msg.content = initPrompt;
                messages.Add(msg);
            }
            else
            {
                // conversational style
                if (prevChats != null && prevChats.Count() > 0 && prevChats[0] != null && prevChats[0].input != null && prevChats[0].output != null)
                {
                    foreach (Chat input in prevChats[0].input)
                    {
                        messages.Add(input);
                    }
                    messages.Add(prevChats[0].output);
                }
                else skipTemplate = true;
            }
            return skipTemplate;
        }
        public string ApplyPromptEnhancers(List<PromptEnhancer>? promptEnhancers, string? promptOutput, ref List<Chat> messages)
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
