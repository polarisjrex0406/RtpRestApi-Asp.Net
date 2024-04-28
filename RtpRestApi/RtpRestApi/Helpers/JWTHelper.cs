using Microsoft.IdentityModel.Tokens;
using RtpRestApi.Entities;
using RtpRestApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RtpRestApi.Helpers
{
    public class JWTHelper
    {
        /*
         * Helper method, for generating Json Web Token
         * 
         * install Nuget System.IdentityModel.Tokens.Jwt
         */
        public static string GenerateJsonWebToken(Admin user, AppSettings settings)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(settings.SecretKey);
            var date = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // makes the properties of the user to be the claim identity, parding user into the token
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user._id),
                    new Claim("name", user.name),
                    new Claim("email", user.email),
                    new Claim("username", user.surname),
                    new Claim("role", user.role)
                }),

                // Set the token expiry to a day - This value is only to show
                Expires = DateTime.UtcNow.AddDays(1),
                NotBefore = date,

                // setting the signing credentials
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            // create the token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static User? DecodeJsonWebTokenToUser(string jsonWebToken, string SecretKey)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(SecretKey);
                tokenHandler.ValidateToken(jsonWebToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    // set clockskew to zero so token expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // Logging Purpose
                Console.WriteLine("Cookie was issued at " + jwtToken.IssuedAt);
                Console.WriteLine("Cookie was valid to " + jwtToken.ValidTo);

                string id = jwtToken.Claims.First(x => x.Type == "id").Value;
                string name = jwtToken.Claims.First(x => x.Type == "name").Value;
                string email = jwtToken.Claims.First(x => x.Type == "email").Value;
                string username = jwtToken.Claims.First(x => x.Type == "username").Value;

                // Return the decoded user from the token
                return new User
                {
                    Id = id,
                    Name = name,
                    Email = email,
                    Username = username,
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
