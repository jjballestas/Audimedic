using Audimedic_Backend.Data;
using Audimedic_Backend.Enums;
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Audimedic_Backend.Models.Entidades;
namespace Audimedic_Backend.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/entidades")]
    [ApiExplorerSettings(GroupName = "admin-v1")]
    [Tags("Entidades (Admin)")]
    [Authorize(Roles = "Admin")]
    public class EntidadesController : ControllerBase
    {
        private static string NormalizeCodigo(string? codigo)
    => (codigo ?? string.Empty).Trim().ToUpperInvariant();

        private readonly AudimedicDbContext _db;
        public EntidadesController(AudimedicDbContext db) => _db = db;

        /// <summary>Lista entidades (opcional: filtro por código/nombre).</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] string? q, CancellationToken ct)
        {
            var query = _db.Entidades.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var nq = q.Trim().ToUpperInvariant();
                query = query.Where(e => e.Codigo.ToUpper() == nq || e.Nombre.Contains(q));
            }

            var items = await query
                .OrderBy(e => e.Nombre)
                .Select(e => new EntidadDto { Id = e.Id, Codigo = e.Codigo, Nombre = e.Nombre, TipoEntidad = e.TipoEntidad })
                .ToListAsync(ct);

            return Ok(items);
        }

        /// <summary>Obtiene una entidad por Id.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var e = await _db.Entidades.FindAsync(new object[] { id }, ct);
            if (e is null) return NotFound();
            return Ok(new EntidadDto { Id = e.Id, Codigo = e.Codigo, Nombre = e.Nombre, TipoEntidad = e.TipoEntidad });
        }

        /// <summary>Crea una entidad.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] EntidadUpsertDto dto, CancellationToken ct)
        {
            var codigo = NormalizeCodigo(dto.Codigo);
            if (string.IsNullOrWhiteSpace(codigo))
                return BadRequest("El código es requerido.");

            // Validación simple de unicidad de Código (opcional)
            var exists = await _db.Entidades.AnyAsync(x => x.Codigo == codigo, ct);
            if (exists) return BadRequest($"Ya existe una entidad con código '{codigo}'.");


            var e = new Entidad
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                TipoEntidad = dto.TipoEntidad
            };
            _db.Entidades.Add(e);
            await _db.SaveChangesAsync(ct);

            var result = new EntidadDto { Id = e.Id, Codigo = e.Codigo, Nombre = e.Nombre, TipoEntidad = e.TipoEntidad };
            return CreatedAtAction(nameof(GetById), new { id = e.Id }, result);
        }





        /// <summary>Actualiza una entidad.</summary>
     
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EntidadUpsertDto dto, CancellationToken ct)
        {
            var e = await _db.Entidades.FindAsync(new object[] { id }, ct);
            if (e is null) return NotFound();

            var codigo = NormalizeCodigo(dto.Codigo);
            if (string.IsNullOrWhiteSpace(codigo))
                return BadRequest("El código es requerido.");

            var exists = await _db.Entidades.AnyAsync(x => x.Codigo == codigo && x.Id != id, ct);
            if (exists) return BadRequest($"Ya existe otra entidad con código '{codigo}'.");

            e.Codigo = codigo;
            e.Nombre = dto.Nombre?.Trim() ?? string.Empty;
            e.TipoEntidad = dto.TipoEntidad;

            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

        /// <summary>Elimina una entidad.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var e = await _db.Entidades.FindAsync(new object[] { id }, ct);
            if (e is null) return NotFound();

            _db.Entidades.Remove(e);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }

    // DTOs (respuesta y entrada)
  
   
}
