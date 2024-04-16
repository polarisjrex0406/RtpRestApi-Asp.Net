using RtpRestApi.Models;
using RtpRestApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RtpRestApi.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminsController : ControllerBase
{
    private readonly AdminsService _adminsService;
    private readonly AdminPasswordsService _adminPasswordsService;

    public AdminsController(AdminsService adminsService, AdminPasswordsService adminPasswordsService)
    {
        _adminsService = adminsService;
        _adminPasswordsService = adminPasswordsService;
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

    private string? CurrentRole()
    {
        string? role = null;
        var userInfo = HttpContext.User.FindFirst(ClaimTypes.Role);
        if (userInfo != null)
        {
            role = userInfo.Value;
        }
        return role;
    }

    [HttpGet]
    [Route("listAll")]
    public async Task<IActionResult> Get()
    {
        if (CurrentRole() != "Owner")
        {
            object? fakeObj = null;
            Response.StatusCode = StatusCodes.Status203NonAuthoritative;
            return new JsonResult(new
            {
                success = false,
                result = fakeObj,
                message = "You cannot access users data",
            });
        }

        var resObj = await _adminsService.GetAsync();
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
        if (CurrentRole() != "Owner")
        {
            object? fakeObj = null;
            Response.StatusCode = StatusCodes.Status203NonAuthoritative;
            return new JsonResult(new
            {
                success = false,
                result = fakeObj,
                pagination = new
                {
                    page = 1,
                    pages = 1,
                    count = 0
                },
                message = "You cannot access users data",
            });
        }

        var resObj = await _adminsService.GetAsync(q, fields);

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
    [Route("search")]
    public async Task<IActionResult> Get([FromQuery] string? q, [FromQuery] string? fields)
    {
        if (CurrentRole() != "Owner")
        {
            object? fakeObj = null;
            Response.StatusCode = StatusCodes.Status203NonAuthoritative;
            return new JsonResult(new
            {
                success = false,
                result = fakeObj,
                pagination = new
                {
                    page = 1,
                    pages = 1,
                    count = 0
                },
                message = "You cannot access users data",
            });
        }

        var resObj = await _adminsService.GetAsync(q, fields);

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
        var resObj = await _adminsService.GetAsync(id);

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
    public async Task<IActionResult> Post([FromBody] AdminRequest newAdminRequest)
    {
        if (newAdminRequest.email == null || newAdminRequest.password == null)
        {
            object? resObj = null;
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new
            {
                success = false,
                result = resObj,
                message = "Email or password fields they don't have been entered."
            });
        }

        if (CurrentRole() == "owner")
        {
            object? resObj = null;
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return new JsonResult(new
            {
                success = false,
                result = resObj,
                message = "you can't create user with role owner"
            });
        }

        // Check if same email and name already exists
        Admin? adminRes = await _adminsService.GetByEmailAsync(newAdminRequest.email);
        if (adminRes == null)
        {
            // Prepare request data for a new account
            Admin adminReq = new Admin();
            adminReq.name = newAdminRequest.name;
            adminReq.surname = newAdminRequest.surname != null ? newAdminRequest.surname : string.Empty;
            adminReq.email = newAdminRequest.email;
            if (newAdminRequest.enabled != null) adminReq.enabled = (bool)newAdminRequest.enabled;
            if (newAdminRequest.role != null) adminReq.role = newAdminRequest.role;

            // Create a new account
            var insertedId = await _adminsService.CreateAsync(adminReq);

            // Save password for new account
            if (insertedId != null)
            {
                await _adminPasswordsService.CreateAsync(insertedId, newAdminRequest.password);

                return Ok(new
                {
                    success = true,
                    result = new
                    {
                        _id = insertedId,
                        enabled = adminReq.enabled,
                        email = adminReq.email,
                        name = adminReq.name,
                        surname = adminReq.surname,
                        photo = adminReq.photo,
                        role = adminReq.role
                    },
                    message = "User document save correctly"
                });
            }
            else
            {
                object? resObj = null;
                Response.StatusCode = StatusCodes.Status403Forbidden;
                return new JsonResult(new
                {
                    success = false,
                    result = resObj,
                    message = "document couldn't save correctly"
                });
            }
        }
        else
        {
            object? resObj = null;
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new
            {
                success = false,
                result = resObj,
                message = "An account with this email already exists."
            });
        }
    }
    
    [HttpPatch]
    [Route("update/{id:length(24)}")]
    public async Task<IActionResult> Update(string id, [FromBody] AdminRequest updatedRequest)
    {
        // Check if same email and name already exists
        Admin? adminRes = await _adminsService.GetByEmailAsync(updatedRequest.email);
        if (adminRes != null)
        {
            // Prepare request data for a new account
            Admin adminReq = new Admin();
            adminReq.name = updatedRequest.name;
            adminReq.surname = updatedRequest.surname != null ? updatedRequest.surname : string.Empty;
            adminReq.email = updatedRequest.email;
            if (updatedRequest.enabled != null) adminReq.enabled = (bool)updatedRequest.enabled;
            if (updatedRequest.role != null) adminReq.role = updatedRequest.role;

            // Update an account
            await _adminsService.UpdateAsync(adminRes._id, adminReq);

            return Ok(new
            {
                success = true,
                result = new
                {
                    _id = adminRes._id,
                    enabled = adminReq.enabled,
                    email = adminReq.email,
                    name = adminReq.name,
                    surname = adminReq.surname,
                    photo = adminReq.photo,
                    role = adminReq.role
                },
                message = "we update this document "
            });
        }
        else
        {
            object? resObj = null;
            Response.StatusCode = StatusCodes.Status404NotFound;
            return new JsonResult(new
            {
                success = false,
                result = resObj,
                message = "No document found "
            });
        }
    }

    [HttpDelete]
    [Route("delete/{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var resObj = await _adminsService.GetByIdAsync(id);

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
            await _adminsService.RemoveAsync(id);
            await _adminPasswordsService.RemoveByUserAsync(id);
            return Ok(new
            {
                success = true,
                result = resObj,
                message = "Successfully Deleted permantely the document ",
            });
        }
    }
}