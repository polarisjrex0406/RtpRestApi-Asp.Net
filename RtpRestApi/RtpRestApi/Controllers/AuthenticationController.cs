using RtpRestApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using RtpRestApi.Helpers;
using RtpRestApi.Services;
using Microsoft.Extensions.Options;

namespace RtpRestApi.Controllers
{
    [ApiController]
/*    [Route("api/[controller]")]*/
    public class AuthenticationController : Controller
    {
        private Services.IAuthenticationService authenticationService;
        private readonly AppSettings _appSettings;
        private readonly AdminsService _adminsService;

        public AuthenticationController(Services.IAuthenticationService authenticationService,
            IOptions<AppSettings> appSettings, AdminsService adminsService)
        {
            this.authenticationService = authenticationService;
            _appSettings = appSettings.Value;
            _adminsService = adminsService;
        }

        // Login
        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> Login([FromQuery] string timestamp, [FromBody] AuthRequest authRequest)
        {
            // Authenticate the user and get the response
/*            AuthResponse? response = authenticationService.Authenticate(authRequest);*/
            Admin? admin = await _adminsService.GetByEmailAsync(authRequest.Email);
            if (admin == null)
            {
                return Unauthorized(new { Message = "Invalid Login Credentials" });
            }

            string token = JWTHelper.GenerateJsonWebToken(admin, _appSettings);

            AuthResponse response = new AuthResponse(admin, token);

            // create the userclaims
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response.Id),

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
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            // now that we have all the necessary objects for our user's identity
            // we can now sign in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

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

        /*
            Logging out the user
        */
        [HttpPost]
        [Route("api/logout")]
        public async Task<IActionResult> Logout()
        {
            // simply call the SightOutAsync method in the HttpContext object to sign out user.
            // this clear's the existing external cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // redirect the user to a route [like a homepage '/']
            return Ok(new { Message = "Logged out successfully" });
        }
    }
}
