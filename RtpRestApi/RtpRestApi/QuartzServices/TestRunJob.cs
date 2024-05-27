using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Quartz;
using RtpRestApi.Models;
using RtpRestApi.Services;

namespace RtpRestApi.QuartzServices
{
    public class TestRunJob : IJob
    {
        private readonly TestsService _testsService;
        private readonly QueuesService _queuesService;
        private readonly CachePromptsService _cachePromptsService;
        private readonly ExperimentsService _experimentsService;
        private readonly ArtifactsService _artifactsService;
        private readonly TopicsService _topicsService;
        private ChatGptService _chatGptService;

        public TestRunJob(TestsService testsService, QueuesService queuesService, CachePromptsService cachePromptsService,
        ExperimentsService experimentsService, ArtifactsService artifactsService, TopicsService topicsService,
        ChatGptService chatGptService)
        {
            _testsService = testsService;
            _queuesService = queuesService;
            _cachePromptsService = cachePromptsService;
            _experimentsService = experimentsService;
            _artifactsService = artifactsService;
            _topicsService = topicsService;
            _chatGptService = chatGptService;
        }

        private void WriteLogs(string logContent)
        {
            DateTime dt = DateTime.Now;
            string str = logContent;

            // Specify the file path for the log file
            string logFilePath = "Mylogfile.log";

            try
            {
                // Check if the log file exists
                if (!File.Exists(logFilePath))
                {
                    // If the file doesn't exist, create it and write the data
                    using (StreamWriter writer = File.CreateText(logFilePath))
                    {
                        writer.WriteLine($"[{dt}]");
                        writer.WriteLine($"{str}");
                    }
                    Console.WriteLine("Log file created and data written.");
                }
                else
                {
                    // If the file exists, append the data to it
                    using (StreamWriter writer = File.AppendText(logFilePath))
                    {
                        writer.WriteLine($"[{dt}]");
                        writer.WriteLine($"{str}");
                    }
                    Console.WriteLine("Data appended to the existing log file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }

        }
        public async Task Execute(IJobExecutionContext context)
        {
            // Create a linked cancellation token source
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
            WriteLogs("266: Called Execute(IJobExecutionContext context)");
            // Perform the job's work, checking for cancellation requests
            await ProcessTestsAsync();
        }
        private async Task ProcessTestsAsync()
        {
            int taskState = 0;
            var testsList = await _testsService.GetAsync();
            TestResponse testForPast = new TestResponse();
            if (testsList != null && testsList.Count > 0)
            {
                foreach (var test in testsList)
                {
                    if (test?.status == "Pending")
                    {
                        testForPast = test;
                        taskState = 1;
                        break;
                    }
                }
            }
            else
            {
                taskState = 2;
            }

            // Analyze, Queue and Run Test
            if (taskState == 0)
            {
                foreach (var testResponse in testsList)
                {

                    if (testResponse != null && testResponse?.status == "New")
                    {
                        // Check if the job has been canceled
                        // Run Test
                        await RunTestAsync(testResponse);
                    }
                }
            }
            else if (taskState == 1)
            {
                // Check if the job has been canceled
                // Run Test
                await RunTestAsync(testForPast);
            }
        }
        private async Task RunTestAsync(TestResponse testRunning)
        {
            TestResponse testResponse = testRunning;
            WriteLogs($"317: Called RunTestAsync({testRunning.testCode})");
            // Perform the long-running test processing logic
            if (testResponse != null && testResponse?.status == "New")
            {
                // Check if the job has been canceled
                // Set Test As Pending
                testResponse.status = "Pending";
                await _testsService.UpdateAsync(testResponse._id, testResponse);
            }
            // Run Test
            if (testResponse != null && testResponse?.status == "Pending")
            {
                testResponse.experiments = new List<ExperimentInResponse>();
                var currentUserId = testResponse?.createdBy;
                TestRequest newTestRequest = new TestRequest();
                newTestRequest.testCode = testResponse?.testCode;
                newTestRequest.topic = testResponse?.topic_req;
                newTestRequest.experiments = new List<ExperimentInRequest>();
                if (testResponse?.experiments_req != null)
                {                    
                    foreach (var strExp in testResponse.experiments_req)
                    {
                        if (strExp != null)
                        {
                            ExperimentInRequest expInReq = new ExperimentInRequest();
                            expInReq.experiment = strExp;
                            newTestRequest.experiments.Add(expInReq);
                        }
                    }
                }

                var topicObj = await _topicsService.GetAsync(currentUserId, newTestRequest.topic);
                if (newTestRequest.experiments != null)
                {
                    foreach (var expId in newTestRequest.experiments)
                    {
                        if (expId == null || expId.experiment == null || currentUserId == null) continue;
                        var expObj = await _experimentsService.GetAsync(currentUserId, expId.experiment);
                        if (expObj == null) continue;
                        if (expObj.templates == null || expObj.style == null) continue;
                        if (expObj.initPrompt == null || expObj.initPrompt.Count() == 0)
                        {
                            if (topicObj?.topicPrompt != null && topicObj?.topicPrompt.Count > 0)
                            {
                                expObj.initPrompt = topicObj?.topicPrompt;
                            }
                            else
                            {
                                expObj.initPrompt = [""];
                            }
                        }

                        ExperimentInResponse expResponse = new ExperimentInResponse();
                        expResponse.style = expObj.style;
                        expResponse.experimentCode = expObj.experimentCode;
                        expResponse.responses = new List<PerPromptResponse>();

                        var templateList = new List<ArtifactResponse>();
                        bool hasInitPrompt = false;
                        foreach (var templateInRequest in expObj.templates)
                        {
                            var templateObj = await _artifactsService.GetAsync(currentUserId, templateInRequest.templateId);
                            if (templateObj == null) continue;
                            templateList.Add(templateObj);
                            hasInitPrompt |= templateObj.promptOutput.Contains("{{TopicPrompt}}");
                        }

                        var initPrompts = new List<string>();
                        if (!hasInitPrompt)
                        {
                            initPrompts = [""];
                        }
                        else
                        {
                            initPrompts = expObj.initPrompt;
                        }

                        foreach (var init_prompt in initPrompts)
                        {
                            List<History> prevChats = new List<History>();
                            PerPromptResponse perPromptResponse = new PerPromptResponse();
                            perPromptResponse.chatHistory = new List<History>();
                            perPromptResponse.initPrompt = init_prompt;
                            foreach (var templateObj in templateList)
                            {
                                History histResponse = new History();
                                Chat chatResponse = new Chat();
                                chatResponse.role = "assistant";

                                if (templateObj == null) continue;
                                histResponse.artifactName = templateObj.name;

                                // Check criteria and determine if Skip current prompt.
                                bool skipPrompt = !_queuesService.CheckCriterias(templateObj?.ruleLogic, templateObj?.rules, init_prompt, templateObj.promptEnhancers, prevChats);
                                // Depend on style, Skip or Refine init prompt from previous chats or its own.
                                List<Chat> messages = new List<Chat>();
                                Chat msg = new Chat();
                                if (_queuesService.SkipArtifactOrGoChat(expObj?.style, ref messages, ref prevChats)) break;
                                // Enhance prompt with variables of current artifact.
                                string enhancedPrompt = "";
                                if (!skipPrompt)
                                {
                                    enhancedPrompt = _queuesService.ApplyPromptEnhancers(templateObj.promptEnhancers, templateObj.promptOutput, init_prompt, ref messages);
                                }
                                // Configure Chat GPT settings as to use for OpenAI API.
                                JObject gptSettings = _queuesService.ConfigGptSettings(templateObj.chatgptSettings);

                                if (!skipPrompt)
                                {
                                    CachePromptResponse? cachedPrompt = null;
                                    if (expObj?.style == "Stand-alone")
                                    {
                                        cachedPrompt = await _cachePromptsService.ReadOneByArtifactAsync(templateObj._id, currentUserId, templateObj.chatgptSettings, enhancedPrompt);
                                    }

                                    if (cachedPrompt == null)
                                    {
                                        JObject payload = gptSettings;
                                        JArray msgAry = new JArray();
                                        foreach (var message in messages)
                                        {
                                            msgAry.Add(new JObject
                                            {
                                                ["role"] = message.role,
                                                ["content"] = message.content
                                            });
                                        }
                                        payload["messages"] = msgAry;
                                        string? generatedText = await _chatGptService.GetChatCompletionAsync(payload.ToString());

                                        if (expObj?.style == "Stand-alone" && templateObj.useCache && generatedText != null)
                                        {
                                            var cacheRequest = new CachePromptRequest();
                                            cacheRequest.template = templateObj._id;
                                            cacheRequest.cacheTimeoutValue = templateObj.cacheTimeoutValue;
                                            cacheRequest.cacheConditions = templateObj.cacheConditions;
                                            cacheRequest.cacheTimeoutUnit = templateObj.cacheTimeoutUnit;
                                            cacheRequest.chatgptSettings = templateObj.chatgptSettings;
                                            cacheRequest.input = enhancedPrompt;
                                            cacheRequest.output = generatedText;
                                            await _cachePromptsService.CreateOneAsync(cacheRequest, currentUserId);
                                        }
                                        // Input
                                        chatResponse.content = generatedText;
                                    }
                                    else
                                    {
                                        chatResponse.content = cachedPrompt.output;
                                    }
                                }
                                else
                                {
                                    chatResponse.content = "";
                                }
                                histResponse.input = messages;
                                histResponse.output = chatResponse;
                                prevChats.Add(histResponse);
                                perPromptResponse.chatHistory.Add(histResponse);
                            }
                            expResponse.responses.Add(perPromptResponse);
                        }
                        testResponse.experiments.Add(expResponse);
                    }
                }

                // Check if the job has been canceled
                // Update test results to DB
                testResponse.status = "Done";
                await _testsService.UpdateAsync(testResponse._id, testResponse);
            }
        }
    }
}
