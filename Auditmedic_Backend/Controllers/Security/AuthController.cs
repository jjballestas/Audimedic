namespace Audimedic_Backend.Controllers
{
    using Audimedic_Backend.Data;
    using Audimedic_Backend.Models;
  
 
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace Auditmedic_Backend.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly AudimedicDbContext _db;
            private readonly ITokenService _tokenService;

            public AuthController(AudimedicDbContext db, ITokenService tokenService)
            {
                _db = db;
                _tokenService = tokenService;
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] LoginRequest request)
            {
                var user = await _db.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Activo);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.ContrasenaHash))
                    return Unauthorized("Credenciales inválidas");

                var token = _tokenService.GenerateToken(user);
                return Ok(new { token, rol = user.Rol.NombreRol });
            }
        }
    }

}
