using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using RtpRestApi.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace RtpRestApi.Helpers
{
    public class CookieHelper
    {

        public static async Task ValidateCookie(CookieValidatePrincipalContext context, string SecretKey)
        {
            var claimsPrincipal = context.Principal;
            if (claimsPrincipal != null)
            {
                // get the tokens
                var claimToken = claimsPrincipal.Claims.First(c => c.Type == "token").Value;

                if (claimToken != null)
                {
                    User? user = JWTHelper.DecodeJsonWebTokenToUser(claimToken, SecretKey);

                    if (user == null)
                    {
                        Console.WriteLine("Cookie has Expired");
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                    else
                    {
                        Console.WriteLine(user.ToString());
                    }
                }
            }
        }
    }
}
