namespace Audimedic_Backend.Services
{
    using Audimedic_Backend.Security;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;


    public class TokenService : ITokenService
        {
            private readonly IConfiguration _config;

            public TokenService(IConfiguration config)
            {
                _config = config;
            }

            public string GenerateToken(Usuario user)
            {
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("uid", user.Id.ToString()),
                new Claim("rol", user.Rol.NombreRol)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(4),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
     
}
