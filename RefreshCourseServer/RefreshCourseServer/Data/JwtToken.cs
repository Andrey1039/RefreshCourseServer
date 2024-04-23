using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace RefreshCourseServer.Data
{
    public static class JwtToken
    {
        // Генерация JWT токена
        public static string GenerateToken(AppUser user, IConfiguration config)
        {
            var claims = new List<Claim>
            {               
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Initials!),
                new(ClaimTypes.Email, user.Email!)
            };

            var jwtToken = new JwtSecurityToken(
                issuer: config.GetValue<string>("Jwt:Issuer")!,
                audience: config.GetValue<string>("Jwt:Audience")!,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:Secret")!)),
                    SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}