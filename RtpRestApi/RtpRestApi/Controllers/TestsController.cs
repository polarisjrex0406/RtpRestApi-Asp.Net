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
    private readonly ExperimentsService _experimentsService;
    private readonly TopicsService _topicsService;

    public TestsController(TestsService testsService, ExperimentsService experimentsService, TopicsService topicsService)
    {
        _testsService = testsService;
        _experimentsService = experimentsService;
        _topicsService = topicsService;
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
        // Analyze, Queue and Run Test
        TestResponse testResponse = new TestResponse();
        testResponse.testCode = newTestRequest.testCode;
        testResponse.topic_req = newTestRequest.topic;
        testResponse.experiments_req = new List<string>();
        if (newTestRequest.experiments != null)
        {
            foreach (var expInReq in newTestRequest.experiments)
            {
                if (expInReq != null && expInReq.experiment != null)
                {
                    testResponse.experiments_req.Add(expInReq.experiment);
                }
            }
        }
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
                ExperimentInResponse expResponse = new ExperimentInResponse();
                expResponse.experimentCode = expObj.experimentCode;
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