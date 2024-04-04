using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RefreshCourseServer.Data
{
    public static class JwtToken
    {
        public static string GenerateToken(IdentityUser user, IConfiguration config)
        {
            var claims = new List<Claim>
            {               
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!)
            };

            var jwtToken = new JwtSecurityToken(
                issuer: config.GetValue<string>("Jwt:Issuer")!,
                audience: config.GetValue<string>("Jwt:Audience")!,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:Secret")!)),
                    SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}