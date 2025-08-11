using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auditmedic_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        [HttpGet("perfil")]
        [Authorize]
        public IActionResult GetPerfil()
        {
            var email = User.Identity?.Name ?? "Desconocido";
            var rol = User.FindFirst("rol")?.Value ?? "Sin rol";

            return Ok(new
            {
                mensaje = "Acceso permitido",
                usuario = email,
                rol
            });
        }
    }
}