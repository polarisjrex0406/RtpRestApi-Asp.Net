using RtpRestApi.Models;
using RtpRestApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RtpRestApi.Controllers;

[ApiController]
[Route("api/test")]
public class TestsController : ControllerBase
{
    private readonly TestsService _testsService;
    private readonly ExperimentsService _experimentsService;
    private readonly ArtifactsService _artifactsService;
    private readonly TopicsService _topicsService;

    private List<TopicResponse> _cacheTopicList;

    public TestsController(TestsService testsService, ExperimentsService experimentsService, ArtifactsService artifactsService, TopicsService topicsService)
    {
        _testsService = testsService;
        _experimentsService = experimentsService;
        _artifactsService = artifactsService;
        _topicsService = topicsService;

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

    private async Task<TestResponse?> AutoPopulate(TestResponse rawTest)
    {
        TestResponse populatedTest = rawTest;
        /// CACHING MECHANISM?
        bool flagCached = false;
        foreach (var cacheObj in _cacheTopicList)
        {
            if (cacheObj._id == rawTest.topicId)
            {
                flagCached = true;
                break;
            }
        }
        if (!flagCached)
        {
            populatedTest.topicObj = await _topicsService.GetAsync(CurrentUserId(), populatedTest.topicId);
            if (populatedTest.topicObj != null)
            {
                _cacheTopicList.Add(populatedTest.topicObj);
            }
        }

        if (populatedTest.experiments != null)
        {
            foreach (var expObj in populatedTest.experiments)
            {
                expObj.experimentObj = await _experimentsService.GetAsync(CurrentUserId(), expObj.experimentId);
                if (expObj.experimentObj != null)
                {
                    expObj.experimentObj.topicObj = await _topicsService.GetAsync(CurrentUserId(), expObj.experimentObj.topicId);
                    flagCached = false;
                    foreach (var cacheObj in _cacheTopicList)
                    {
                        if (cacheObj._id == expObj.experimentObj.topicId)
                        {
                            flagCached = true;
                            break;
                        }
                    }
                    if (!flagCached)
                    {
                        expObj.experimentObj.topicObj = await _topicsService.GetAsync(CurrentUserId(), expObj.experimentObj.topicId);
                        if (expObj.experimentObj.topicObj != null)
                        {
                            _cacheTopicList.Add(expObj.experimentObj.topicObj);
                        }
                    }

                    if (expObj.experimentObj.templates != null)
                    {
                        foreach (var artiObj in expObj.experimentObj.templates)
                        {
                            artiObj.templateObj = await _artifactsService.GetAsync(CurrentUserId(), artiObj.templateId);
                        }
                    }
                }
            }
        }

        return populatedTest;
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
            for (int i = 0; i < testsList.Count; i ++)
            {
                var testObj = testsList[i];
                if (testObj == null) { continue; }
                var popObj = await AutoPopulate(testObj);
                if (popObj == null) { continue; }
                testsList[i] = popObj;
            }

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
            for (int i = 0; i < testsList.Count; i++)
            {
                var testObj = testsList[i];
                if (testObj == null) { continue; }
                var popObj = await AutoPopulate(testObj);
                if (popObj == null) { continue; }
                testsList[i] = popObj;
            }

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
            for (int i = 0; i < testsList.Count; i++)
            {
                var testObj = testsList[i];
                if (testObj == null) { continue; }
                var popObj = await AutoPopulate(testObj);
                if (popObj == null) { continue; }
                testsList[i] = popObj;
            }
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
            var popObj = await AutoPopulate(testObj);
            if (popObj != null) {
                testObj = popObj;
            }
            return Ok(new
            {
                success = true,
                result = testObj,
                message = "we found this document",
            });
        }
    }
    /*
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Post([FromBody] ExperimentRequest newExperimentRequest)
    {
        var resObj = await _experimentsService.CreateAsync(CurrentUserId(), newExperimentRequest);
        if (resObj != null)
        {
            resObj.topicObj = await _topicsService.GetAsync(CurrentUserId(), resObj.topicId);
            if (resObj.templates != null)
            {
                foreach (var resItem in resObj.templates)
                {
                    resItem.templateObj = await _artifactsService.GetAsync(CurrentUserId(), resItem.templateId);
                }
            }
        }

        return Ok(new
        {
            success = true,
            result = resObj,
            message = "Successfully Created the document in Model",
        });
    }
    */
    [HttpPatch]
    [Route("update/{id:length(24)}")]
    public async Task<IActionResult> Update(string id, [FromBody] TestRequest updatedTest)
    {
        var res = await _testsService.UpdateAsync(id, updatedTest);

        if (res is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return new JsonResult(new
            {
                success = false,
                result = res,
                message = "No document found"
            });
        }
        else
        {
            var testObj = await _testsService.GetAsync(CurrentUserId(), id);
            if (testObj != null)
            {
                var popObj = await AutoPopulate(testObj);
                if (popObj != null)
                {
                    testObj = popObj;
                }
            }

            return Ok(new
            {
                success = true,
                result = testObj,
                message = "we update this document",
            });
        }
    }

    [HttpDelete]
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
            var popObj = await AutoPopulate(testObj);
            if (popObj != null)
            {
                testObj = popObj;
            }

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