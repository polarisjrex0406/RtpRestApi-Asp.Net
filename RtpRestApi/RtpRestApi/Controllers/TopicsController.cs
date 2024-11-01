﻿using RtpRestApi.Models;
using RtpRestApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RtpRestApi.Controllers;

[ApiController]
[Route("api/topic")]
[Authorize]
public class TopicsController : ControllerBase
{
    private readonly TopicsService _topicsService;
    private readonly ArtifactsService _artifactsService;
    private readonly ExperimentsService _experimentService;

    public TopicsController(TopicsService topicsService, ArtifactsService artifactsService, ExperimentsService experimentService)
    {
        _topicsService = topicsService;
        _artifactsService = artifactsService;
        _experimentService = experimentService;
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
        var resObj = await _topicsService.GetAsync();
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
        var resObj = await _topicsService.GetAsync(CurrentUserId(), q, fields);

        if (resObj == null)
        {
            return NoContent();
        }

        if (resObj.Count == 0) {
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

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Get([FromQuery] string? q, [FromQuery] string? fields)
    {
        var resObj = await _topicsService.GetAsync(CurrentUserId(), q, fields);

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

    [HttpGet]
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

    [HttpGet]
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

            var artiList = await _artifactsService.GetAsync(CurrentUserId(), id, "topic");
            if (artiList != null)
            {
                foreach (var arti in artiList)
                {
                    if (arti?._id == null) continue;
                    await _artifactsService.RemoveAsync(arti?._id);
                }
            }

            var expList = await _experimentService.GetAsync(CurrentUserId(), id, "topic");
            if (expList != null)
            {
                foreach (var exp in expList)
                {
                    if (exp?._id == null) continue;
                    await _experimentService.RemoveAsync(exp?._id);
                }
            }

            return Ok(new
            {
                success = true,
                result = resObj,
                message = "Successfully Deleted the document",
            });
        }
    }
}