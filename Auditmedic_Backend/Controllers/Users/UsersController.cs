using Audimedic_Backend.Data;
using Audimedic_Backend.Models.Users;
using Audimedic_Backend.Services.Historias;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace Audimedic_Backend.Controllers.Users
{
    [ApiController]
    [Route("api/users")]
    [ApiExplorerSettings(GroupName = "users-v1")]
    [Tags("Usuarios (Área Users)")]
    [Authorize(Roles = "Medico,User")]
    public class UsersController : ControllerBase
    {
        private readonly AudimedicDbContext _db;


        
        private readonly IHistoriaUploadService _uploadService;

        public UsersController(AudimedicDbContext db, IHistoriaUploadService uploadService)
        {
            _db = db;
            _uploadService = uploadService;
        }


       




        /// <summary>Obtiene el perfil del usuario autenticado.</summary>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _db.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user is null)
                return NotFound();

            var dto = new UserProfileDto
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol.NombreRol
            };

            return Ok(dto);
        }

        /// <summary>Actualiza el perfil del usuario autenticado (excepto email y rol).</summary>
        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto dto, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _db.Usuarios.FindAsync(new object[] { userId }, ct);
            if (user is null)
                return NotFound();

            user.Nombre = dto.Nombre?.Trim() ?? user.Nombre;

            await _db.SaveChangesAsync(ct);
            return NoContent();
        }


        [HttpPut("me/password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken ct)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("Las contraseñas no coinciden.");

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _db.Usuarios.FindAsync(new object[] { userId }, ct);
            if (user is null)
                return NotFound();

            // Si el usuario tiene contraseña local, validamos la actual
            if (!string.IsNullOrEmpty(user.ContrasenaHash))
            {
                var ok = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword ?? string.Empty, user.ContrasenaHash);
                if (!ok) return BadRequest("La contraseña actual no es correcta.");
            }
            else
            {
                // Si usas cuentas solo-Google y permites establecer password por primera vez:
                // podrías exigir CurrentPassword vacío o una verificación adicional.
                if (!string.IsNullOrEmpty(dto.CurrentPassword))
                    return BadRequest("Este usuario no tiene contraseña local configurada.");
            }

            user.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

    }
}
