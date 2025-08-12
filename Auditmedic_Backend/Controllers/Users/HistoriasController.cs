using Audimedic_Backend.Data;
using Audimedic_Backend.Models.Historias;
using Audimedic_Backend.Services.Historias;
using Audimedic_Backend.Services.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Audimedic_Backend.Controllers.Users
{
    [ApiController]
    [Route("api/users/historias")]
    [Authorize]
    public class HistoriasController : ControllerBase
    {
        private readonly IHistoriaUploadService _uploadService;
        private readonly AudimedicDbContext _db;
        private readonly IPrivateStorage _storage;

        public HistoriasController(
            IHistoriaUploadService uploadService,
            AudimedicDbContext db,
            IPrivateStorage storage)
        {
            _uploadService = uploadService;
            _db = db;
            _storage = storage;
        }

        /// <summary>
        /// Carga un PDF/imagen y crea la historia por médico vinculada a la historia compartida (Entidad+Número).
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(100_000_000)] // 100MB (ajusta)
        public async Task<IActionResult> Upload([FromForm] UploadHistoriaDto dto, IFormFile file, CancellationToken ct)
        {
            if (file == null) return BadRequest("Archivo requerido.");

            var (id, ruta) = await _uploadService.SubirAsync(file, dto, ct);
            return Ok(new { historiaClinicaMedicoId = id, ruta });
        }

        /// <summary>
        /// Descarga el último archivo asociado a la historia (endpoint protegido).
        /// </summary>
        [HttpGet("{historiaClinicaMedicoId:int}/archivo")]
        public async Task<IActionResult> DescargarArchivo([FromRoute] int historiaClinicaMedicoId, CancellationToken ct)
        {
            // Obtener historia y archivos
            var historia = await _db.HistoriasClinicasMedico
                .Include(h => h.HistoriaCompartida)
                    .ThenInclude(c => c.Archivos)
                .Include(h => h.Medico).ThenInclude(m => m.Usuario)
                .FirstOrDefaultAsync(h => h.Id == historiaClinicaMedicoId, ct);

            if (historia == null) return NotFound("Historia no encontrada.");

            // (Opcional/recomendado) Validar que el usuario actual sea el dueño o tenga rol admin
            // var currentEmail = User.Identity?.Name;
            // if (historia.Medico.Usuario.Email != currentEmail && !User.IsInRole("admin")) return Forbid();

            var archivo = historia.HistoriaCompartida.Archivos
                .OrderByDescending(a => a.FechaSubida)
                .FirstOrDefault();

            if (archivo == null) return NotFound("Historia sin archivos.");

            var physical = _storage.GetPhysicalPath(archivo.RutaArchivo);
            if (!System.IO.File.Exists(physical)) return NotFound("Archivo no existe en almacenamiento.");

            var contentType = Path.GetExtension(physical).ToLower() == ".pdf" ? "application/pdf" : "image/*";
            var stream = System.IO.File.OpenRead(physical);
            return File(stream, contentType, fileDownloadName: archivo.NombreArchivo);
        }
    }
}
