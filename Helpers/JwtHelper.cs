using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InvoiceApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace InvoiceApi.Helpers
{
    public static class JwtHelper
    {
        public static JwtTokenResult GenerateToken(AppUser user, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(double.Parse(jwtSettings["ExpirationMinutes"] ?? "60"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                NotBefore = now,
                IssuedAt = now,
                Expires = expires,
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new JwtTokenResult
            {
                Token = tokenString,
                ExpiresAt = expires
            };
        }
    }
}
