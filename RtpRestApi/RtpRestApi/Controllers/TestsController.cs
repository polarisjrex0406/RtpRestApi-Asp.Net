using RtpRestApi.Models;
using RtpRestApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace RtpRestApi.Controllers;

[ApiController]
[Route("api/test")]
[Authorize]
public class TestsController : ControllerBase
{
    private readonly TestsService _testsService;
    private readonly QueuesService _queuesService;
    private readonly CachePromptsService _cachePromptsService;
    private readonly ExperimentsService _experimentsService;
    private readonly ArtifactsService _artifactsService;
    private readonly TopicsService _topicsService;
    private ChatGptService _chatGptService;

    private List<TopicResponse> _cacheTopicList;

    public TestsController(TestsService testsService, QueuesService queuesService, CachePromptsService cachePromptsService,
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

        _cacheTopicList = new List<TopicResponse>();
    }

    private string? CurrentUserId()
    {
        string? userId = null;
        var userInfo = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userInfo != null)
        {
            userId = userInfo.Value;
        }
        return userId;
    }

    [HttpGet]
    [Route("listAll")]
    public async Task<IActionResult> Get()
    {
        var testsList = await _testsService.GetAsync();
        if (testsList == null)
        {
            return NoContent();
        }

        if (testsList.Count == 0)
        {
            Response.StatusCode = StatusCodes.Status203NonAuthoritative;
            return new JsonResult(new
            {
                success = false,
                result = testsList,
                message = "Collection is Empty",
            });
        }
        else
        {
            return Ok(new
            {
                success = true,
                result = testsList,
                message = testsList.Count > 0 ? "Successfully found all documents" : "Collection is Empty",
            });
        }
    }
    
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> Get([FromQuery] int? page, [FromQuery] int? items, [FromQuery] string? q, [FromQuery] string? fields)
    {
        var testsList = await _testsService.GetAsync(CurrentUserId(), q, fields);

        if (testsList == null)
        {
            return NoContent();
        }

        if (testsList.Count == 0)
        {
            Response.StatusCode = StatusCodes.Status203NonAuthoritative;
            return new JsonResult(new
            {
                success = true,
                result = testsList,
                pagination = new
                {
                    page = 1,
                    pages = 1,
                    count = testsList.Count
                },
                message = "Collection is Empty",
            });
        }
        else
        {
            return Ok(new
            {
                success = true,
                result = testsList,
                pagination = new
                {
                    page = 1,
                    pages = 1,
                    count = testsList.Count
                },
                message = "Successfully found all documents",
            });
        }
    }
    
    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Get([FromQuery] string? q, [FromQuery] string? fields)
    {
        var testsList = await _testsService.GetAsync(CurrentUserId(), q, fields);

        if (testsList == null)
        {
            return NoContent();
        }

        if (testsList.Count == 0)
        {
            Response.StatusCode = StatusCodes.Status203NonAuthoritative;
            return new JsonResult(new
            {
                success = true,
                result = testsList,
                pagination = new
                {
                    page = 1,
                    pages = 1,
                    count = testsList.Count
                },
                message = "Collection is Empty",
            });
        }
        else
        {
            return Ok(new
            {
                success = true,
                result = testsList,
                pagination = new
                {
                    page = 1,
                    pages = 1,
                    count = testsList.Count
                },
                message = "Successfully found all documents",
            });
        }
    }
        
    [HttpGet]
    [Route("read/{id:length(24)}")]
    public async Task<ActionResult> Get(string id)
    {
        var testObj = await _testsService.GetAsync(CurrentUserId(), id);

        if (testObj == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return new JsonResult(new
            {
                success = false,
                result = testObj,
                message = "No document found"
            });
        }
        else
        {
            return Ok(new
            {
                success = true,
                result = testObj,
                message = "we found this document",
            });
        }
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Post([FromBody] TestRequest newTestRequest)
    {
        // Populate from new TestRequest
        // Anaylze, Queue and Run Test
        TestResponse testResponse = new TestResponse();
        testResponse.testCode = newTestRequest.testCode;
        testResponse.createdBy = CurrentUserId();
        testResponse.experiments = new List<ExperimentInResponse>();
        var topicObj = await _topicsService.GetAsync(CurrentUserId(), newTestRequest.topic);
        testResponse.topicName = topicObj?.name;
        testResponse.topicPrompt = topicObj?.topicPrompt;
        if (newTestRequest.experiments != null)
        {
            foreach (var expId in newTestRequest.experiments)
            {
                var expObj = await _experimentsService.GetAsync(CurrentUserId(), expId.experiment);
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

                foreach (var init_prompt in expObj.initPrompt)
                {
                    List<History> prevChats = new List<History>();
                    PerPromptResponse perPromptResponse = new PerPromptResponse();
                    perPromptResponse.chatHistory = new List<History>();
                    perPromptResponse.initPrompt = init_prompt;
                    foreach (var template in expObj.templates)
                    {
                        History histResponse = new History();
                        Chat chatResponse = new Chat();
                        chatResponse.role = "assistant";

                        var templateObj = await _artifactsService.GetAsync(CurrentUserId(), template.templateId);
                        if (templateObj == null) continue;
                        histResponse.artifactName = templateObj.name;

                        // Check criteria and determine if Skip current prompt.
                        bool skipPrompt = !_queuesService.CheckCriterias(expObj?.ruleLogic, expObj?.rules, init_prompt, templateObj.promptEnhancers);
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
                                cachedPrompt = await _cachePromptsService.ReadOneByArtifactAsync(template.templateId, CurrentUserId());
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
                                    cacheRequest.template = template.templateId;
                                    cacheRequest.cacheTimeoutValue = templateObj.cacheTimeoutValue;
                                    cacheRequest.cacheConditions = templateObj.cacheConditions;
                                    cacheRequest.cacheTimeoutUnit = templateObj.cacheTimeoutUnit;
                                    cacheRequest.initPrompt = init_prompt;
                                    cacheRequest.input = enhancedPrompt;
                                    cacheRequest.output = generatedText;
                                    await _cachePromptsService.CreateOneAsync(cacheRequest, CurrentUserId());
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
                        // Has no {{TopicPrompt}} in Prompt Output
                        if (!templateObj.promptOutput.Contains("{{TopicPrompt}}")) {
                            perPromptResponse.initPrompt = "";
                        }
                    }
                    expResponse.responses.Add(perPromptResponse);
                    // Has no {{TopicPrompt}} in Prompt Output
                    if (perPromptResponse.initPrompt == "") break;
                }
                testResponse.experiments.Add(expResponse);
            }
        }

        // Save test results to DB
        await _testsService.CreateAsync(testResponse);

        return Ok(new
        {
            success = true,
            message = "Test created successfully",
        });
    }

    [HttpGet]
    [Route("delete/{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var testObj = await _testsService.GetAsync(CurrentUserId(), id);

        if (testObj == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return new JsonResult(new
            {
                success = false,
                result = testObj,
                message = "No document found"
            });
        }
        else
        {
            await _testsService.RemoveAsync(id);
            return Ok(new
            {
                success = true,
                result = testObj,
                message = "Successfully Deleted the document",
            });
        }
    }
}