using Audimedic_Backend.Data;
using Audimedic_Backend.Enums;
using Audimedic_Backend.Models.Historias;
using Audimedic_Backend.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Audimedic_Backend.Services.Historias
{

    public class HistoriaUploadService : IHistoriaUploadService
    {
        private readonly AudimedicDbContext _db;
        private readonly IPrivateStorage _storage;

        public HistoriaUploadService(AudimedicDbContext db, IPrivateStorage storage)
        {
            _db = db;
            _storage = storage;
        }

        public async Task<(int historiaClinicaMedicoId, string rutaRelativa)> SubirAsync(
            IFormFile archivo,
            UploadHistoriaDto dto,
            CancellationToken ct = default)
        {
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("El archivo no puede estar vacío.", nameof(archivo));

            // 1) Buscar entidad por “código” (en este proyecto usamos Nombre como código visible)
            var entidad = await _db.Entidades
                .FirstOrDefaultAsync(e => e.Nombre == dto.CodigoEntidad, ct);
            if (entidad == null)
                throw new InvalidOperationException($"Entidad '{dto.CodigoEntidad}' no existe.");

            // 2) Obtener/crear HistoriaCompartida por (EntidadId, NumeroHistoria)
            var compartida = await _db.HistoriasCompartidas
                .FirstOrDefaultAsync(h => h.EntidadId == entidad.Id && h.NumeroHistoria == dto.NumeroHistoria, ct);

            if (compartida == null)
            {
                compartida = new HistoriaCompartida
                {
                    EntidadId = entidad.Id,
                    NumeroHistoria = dto.NumeroHistoria
                };
                _db.HistoriasCompartidas.Add(compartida);
                await _db.SaveChangesAsync(ct);
            }

            // 3) Guardar UNA sola copia del archivo en almacenamiento privado (ruta relativa)
            var carpetaRelativa = $"{dto.CodigoEntidad}/{dto.NumeroHistoria}";
            string rutaRelativa;
            using (var stream = archivo.OpenReadStream())
            {
                rutaRelativa = await _storage.SaveAsync(stream, archivo.FileName, carpetaRelativa, ct);
            }

            var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            var tipo = ext == ".pdf" ? TipoArchivoHistoria.PDF : TipoArchivoHistoria.Imagen;

            _db.ArchivosHistoriasClinicas.Add(new ArchivoHistoriaClinica
            {
                HistoriaCompartidaId = compartida.Id,
                NombreArchivo = archivo.FileName,
                RutaArchivo = rutaRelativa,   // ← guardamos solo relativo
                TipoArchivo = tipo,
                FechaSubida = DateTime.UtcNow
            });

            // 4) Crear la instancia por médico
            var historiaMedico = new HistoriaClinicaMedico
            {
                MedicoId = dto.MedicoId,
                HistoriaCompartidaId = compartida.Id,
                Estado = EstadoHistoria.Pendiente,
                FechaCirugia = dto.FechaCirugia,
                HoraCirugia = dto.HoraCirugia,
                TiempoQuirurgico = dto.TiempoQuirurgico,
                TiempoAnestesico = dto.TiempoAnestesico,
                Sala = dto.Sala
            };

            _db.HistoriasClinicasMedico.Add(historiaMedico);
            await _db.SaveChangesAsync(ct);

            return (historiaMedico.Id, rutaRelativa);
        }
    }

}
