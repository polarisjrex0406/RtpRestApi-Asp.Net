using RtpRestApi.Models;
using RtpRestApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RtpRestApi.Controllers;

[ApiController]
[Route("api/template")]
public class ArtifactsController : ControllerBase
{
    private readonly ArtifactsService _artifactsService;
    private readonly TopicsService _topicsService;

    public ArtifactsController(ArtifactsService artifactsService, TopicsService topicsService)
    {
        _artifactsService = artifactsService;
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
        var resObj = await _artifactsService.GetAsync();
        if (resObj == null)
        {
            return NoContent();
        }

        if (resObj.Count == 0)
        {
            Response.StatusCode = StatusCodes.Status203NonAuthoritative;
            return new JsonResult(new
            {
                success = false,
                result = resObj,
                message = "Collection is Empty",
            });
        }
        else
        {
            foreach (var resItem in  resObj)
            {
                resItem.topicObj = await _topicsService.GetAsync(CurrentUserId(), resItem.topicId);
            }

            return Ok(new
            {
                success = true,
                result = resObj,
                message = resObj.Count > 0 ? "Successfully found all documents" : "Collection is Empty",
            });
        }
    }

    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> Get([FromQuery] int? page, [FromQuery] int? items, [FromQuery] string? q, [FromQuery] string? fields)
    {
        var resObj = await _artifactsService.GetAsync(CurrentUserId(), q, fields);

        if (resObj == null)
        {
            return NoContent();
        }

        if (resObj.Count == 0)
        {
            Response.StatusCode = StatusCodes.Status203NonAuthoritative;
            return new JsonResult(new
            {
                success = true,
                result = resObj,
                pagination = new
                {
                    page = 1,
                    pages = 1,
                    count = resObj.Count
                },
                message = "Collection is Empty",
            });
        }
        else
        {
            foreach (var resItem in resObj)
            {
                resItem.topicObj = await _topicsService.GetAsync(CurrentUserId(), resItem.topicId);
            }

            return Ok(new
            {
                success = true,
                result = resObj,
                pagination = new
                {
                    page = 1,
                    pages = 1,
                    count = resObj.Count
                },
                message = "Successfully found all documents",
            });
        }
    }

/*    [HttpGet]
    [Route("read/{id:length(24)}")]
    public async Task<ActionResult> Get(string id)
    {
        var resObj = await _topicsService.GetAsync(CurrentUserId(), id);

        if (resObj == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return new JsonResult(new
            {
                success = false,
                result = resObj,
                message = "No document found"
            });
        }
        else
        {
            return Ok(new
            {
                success = true,
                result = resObj,
                message = "we found this document",
            });
        }
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Post([FromBody] TopicRequest newTopicRequest)
    {
        TopicResponse? res = await _topicsService.CreateAsync(CurrentUserId(), newTopicRequest);

        return Ok(new
        {
            success = true,
            result = res,
            message = "Successfully Created the document in Model",
        });
    }

    [HttpPatch]
    [Route("update/{id:length(24)}")]
    public async Task<IActionResult> Update(string id, [FromBody] TopicRequest updatedTopic)
    {
        var res = await _topicsService.UpdateAsync(id, updatedTopic);

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
            var topic = await _topicsService.GetAsync(CurrentUserId(), id);

            return Ok(new
            {
                success = true,
                result = topic,
                message = "we update this document",
            });
        }
    }

    [HttpDelete]
    [Route("delete/{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var resObj = await _topicsService.GetAsync(CurrentUserId(), id);

        if (resObj == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return new JsonResult(new
            {
                success = false,
                result = resObj,
                message = "No document found"
            });
        }
        else
        {
            await _topicsService.RemoveAsync(id);
            return Ok(new
            {
                success = true,
                result = resObj,
                message = "Successfully Deleted the document",
            });
        }
    }*/
}