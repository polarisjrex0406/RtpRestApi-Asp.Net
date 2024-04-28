using RtpRestApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using RtpRestApi.Helpers;
using RtpRestApi.Services;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using RtpRestApi.Entities;
using Microsoft.AspNetCore.Authorization;

namespace RtpRestApi.Controllers
{
    [ApiController]
/*    [Route("api/[controller]")]*/
    public class AuthenticationController : Controller
    {
        private Services.IAuthenticationService authenticationService;
        private readonly AppSettings _appSettings;
        private readonly AdminsService _adminsService;
        private readonly AdminPasswordsService _adminPasswordsService;

        public AuthenticationController(Services.IAuthenticationService authenticationService,
            IOptions<AppSettings> appSettings, AdminsService adminsService,
            AdminPasswordsService adminPasswordsService)
        {
            this.authenticationService = authenticationService;
            _appSettings = appSettings.Value;
            _adminsService = adminsService;
            _adminPasswordsService = adminPasswordsService;
        }

        // Login
        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> Login([FromQuery] string timestamp, [FromBody] AuthRequest authRequest)
        {
            // Authenticate the user and get the response
            /*            AuthResponse? response = authenticationService.Authenticate(authRequest);*/
            Admin? admin;
            try
            {
                admin = await _adminsService.GetByEmailAsync(authRequest.Email);
                if (admin == null)
                {
                    Response.StatusCode = StatusCodes.Status404NotFound;
                    object? resObj = null;
                    return new JsonResult(new
                    {
                        success = false,
                        result = resObj,
                        message = "No account with this email has been registered."
                    });
                }
                else
                {
                    if (!admin.enabled)
                    {
                        Response.StatusCode = StatusCodes.Status409Conflict;
                        object? resObj = null;
                        return new JsonResult(new
                        {
                            success = false,
                            result = resObj,
                            message = "Your account is disabled, contact your account adminstrator"
                        });
                    }
                    bool passwordValid = false;

                    AdminPassword? adminPassword = await _adminPasswordsService.GetByUserAsync(admin._id);
                    if (adminPassword != null)
                    {
                        passwordValid = _adminPasswordsService.ValidatePassword(authRequest.Password, 
                            adminPassword.password);
                    }

                    if (!passwordValid)
                    {
                        Response.StatusCode = StatusCodes.Status403Forbidden;
                        object? resObj = null;
                        return new JsonResult(new
                        {
                            success = false,
                            result = resObj,
                            message = "Invalid credentials."
                        });
                    }
                }

                string token = JWTHelper.GenerateJsonWebToken(admin, _appSettings);

                AuthResponse response = new AuthResponse(admin, token);

                // create the userclaims
                var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response.Id),
                new Claim(ClaimTypes.Role, response.Role),

                // specify custom claims
                new Claim("token", response.Token)
            };

                // creating the identity
                var identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                // creating the principal object with the identity
                var principal = new ClaimsPrincipal(identity);

                // settings for the authentication properties
                var authProperties = new AuthenticationProperties
                {
                    // AllowRefresh = true,
                    // Refreshing the authentication session should be allowed.

                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = "http://localhost:3000/login"
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                // now that we have all the necessary objects for our user's identity
                // we can now sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
            }
            catch (Exception ex)
            {
                return Ok(new { Message = ex.ToString() });
            }

            return Ok(new {
                success = true,
                result = new {
                    id = admin._id,
                    name = admin.name,
                    surname = admin.surname,
                    role = admin.role,
                    email = admin.email,
                },
                message = "Successfully login user",
            });
        }

        // Logging out the user
        [HttpPost]
        [Route("api/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // simply call the SightOutAsync method in the HttpContext object to sign out user.
            // this clear's the existing external cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // redirect the user to a route [like a homepage '/']
            return Ok(new
            {
                success = true,
                result = new { },
                message = "Logged out successfully"
            });
        }

        // Register new user
        [HttpPost]
        [Route("api/register")]
        public async Task<IActionResult> Register([FromBody] AdminRequest newAdminToRegist)
        {
            // Break full name into first name and last name
            string fullname = newAdminToRegist.name.Trim();
            int lastSpaceIndex = fullname.LastIndexOf(' ');
            string firstname = lastSpaceIndex > 0 ? fullname.Substring(0, lastSpaceIndex) : fullname;
            string lastname = lastSpaceIndex > 0 ? fullname.Substring(lastSpaceIndex + 1, fullname.Length - lastSpaceIndex - 1) : string.Empty;

            // Check if same email and name already exists
            Admin? adminRes = await _adminsService.GetByEmailAsync(newAdminToRegist.email);
            if (adminRes == null)
            {
                // Prepare request data for a new account
                Admin adminReq = new Admin();
                adminReq.name = firstname;
                adminReq.surname = lastname;
                adminReq.email = newAdminToRegist.email;
                // Create a new account
                var insertedId = await _adminsService.CreateAsync(adminReq);
                // Save password for new account
                if (insertedId != null)
                {
                    await _adminPasswordsService.CreateAsync(insertedId, newAdminToRegist.password, BCrypt.Net.BCrypt.GenerateSalt());

                    return Ok(new
                    {
                        success = true,
                        result = new { },
                        message = "account registered successfully."
                    });
                }
                else
                {
                    return Unauthorized(new
                    {
                        success = false,
                        result = new { },
                        message = "account registered failed."
                    });
                }
            }
            else
            {
                object? resObj = null;
                if (!adminRes.enabled)
                {
                    Response.StatusCode = StatusCodes.Status409Conflict;                    
                    return new JsonResult(new
                    {
                        success = false,
                        result = resObj,
                        message = "your account is disabled, contact your account adminstrator"
                    });
                }

                Response.StatusCode = StatusCodes.Status409Conflict;
                return new JsonResult(new
                {
                    success = false,
                    result = resObj,
                    message = "account with same email already exists"
                });
            }
        }
    }
}
