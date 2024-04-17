using RtpRestApi.Models;
using RtpRestApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Authorization;

namespace RtpRestApi.Controllers;

[ApiController]
[Route("api/experiment")]
[Authorize]
public class ExperimentsController : ControllerBase
{
    private readonly ExperimentsService _experimentsService;
    private readonly ArtifactsService _artifactsService;
    private readonly TopicsService _topicsService;

    public ExperimentsController(ExperimentsService experimentsService, ArtifactsService artifactsService, TopicsService topicsService)
    {
        _experimentsService = experimentsService;
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
        var resObj = await _experimentsService.GetAsync();
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
            foreach (var resItem in resObj)
            {
                resItem.topicObj = await _topicsService.GetAsync(CurrentUserId(), resItem.topicId);
                if (resItem.templates != null)
                {
                    foreach (var resArtifact in resItem.templates)
                    {
                        resArtifact.templateObj = await _artifactsService.GetAsync(CurrentUserId(), resArtifact.templateId);
                    }
                }
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
        var resObj = await _experimentsService.GetAsync(CurrentUserId(), q, fields);

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
                if (resItem.templates != null)
                {
                    foreach (var resArtifact in resItem.templates)
                    {
                        resArtifact.templateObj = await _artifactsService.GetAsync(CurrentUserId(), resArtifact.templateId);
                    }
                }
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

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Get([FromQuery] string? q, [FromQuery] string? fields)
    {
        var resObj = await _experimentsService.GetAsync(CurrentUserId(), q, fields);

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
                if (resItem.templates != null)
                {
                    foreach (var resArtifact in resItem.templates)
                    {
                        resArtifact.templateObj = await _artifactsService.GetAsync(CurrentUserId(), resArtifact.templateId);
                    }
                }
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


    [HttpGet]
    [Route("read/{id:length(24)}")]
    public async Task<ActionResult> Get(string id)
    {
        var resObj = await _experimentsService.GetAsync(CurrentUserId(), id);

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
            resObj.topicObj = await _topicsService.GetAsync(CurrentUserId(), resObj.topicId);
            if (resObj.templates != null)
            {
                foreach (var resItem in resObj.templates)
                {
                    resItem.templateObj = await _artifactsService.GetAsync(CurrentUserId(), resItem.templateId);
                }
            }
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

    [HttpPatch]
    [Route("update/{id:length(24)}")]
    public async Task<IActionResult> Update(string id, [FromBody] ExperimentRequest updatedExperiment)
    {
        var res = await _experimentsService.UpdateAsync(id, updatedExperiment);

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
            var expObj = await _experimentsService.GetAsync(CurrentUserId(), id);
            if (expObj != null)
            {
                expObj.topicObj = await _topicsService.GetAsync(CurrentUserId(), expObj.topicId);
                if (expObj.templates != null)
                {
                    foreach (var resItem in expObj.templates)
                    {
                        resItem.templateObj = await _artifactsService.GetAsync(CurrentUserId(), resItem.templateId);
                    }
                }
            }

            return Ok(new
            {
                success = true,
                result = expObj,
                message = "we update this document",
            });
        }
    }

    [HttpDelete]
    [Route("delete/{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var resObj = await _experimentsService.GetAsync(CurrentUserId(), id);
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
            await _experimentsService.RemoveAsync(id);
            return Ok(new
            {
                success = true,
                result = resObj,
                message = "Successfully Deleted the document",
            });
        }
    }
}