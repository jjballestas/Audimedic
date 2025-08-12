namespace Audimedic_Backend.Services.Users
{
    using Audimedic_Backend.Data;
    using Microsoft.EntityFrameworkCore;
    public class HistoriaQueryService : IHistoriaQueryService
    {
        private readonly AudimedicDbContext _db;
        private readonly IWebHostEnvironment _env;

        public HistoriaQueryService(AudimedicDbContext db, IWebHostEnvironment env)
        {
            _db = db; _env = env;
        }

        public async Task<(IEnumerable<HistoriaClinicaMedico> items, int total)>
            GetMisHistoriasAsync(int medicoId, int page, int pageSize, string? estado, CancellationToken ct)
        {
            var q = _db.HistoriasClinicasMedico
                .Include(h => h.HistoriaCompartida)
                .ThenInclude(c => c.Entidad)
                .AsQueryable()
                .Where(h => h.MedicoId == medicoId);

            if (!string.IsNullOrWhiteSpace(estado))
                q = q.Where(h => h.Estado.Equals( estado));

            var total = await q.CountAsync(ct);
            var items = await q
                .OrderByDescending(h => h.FechaCarga)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<(string filePath, string contentType, string downloadName)>
            GetArchivoAsync(int historiaClinicaMedicoId, CancellationToken ct)
        {
            var historia = await _db.HistoriasClinicasMedico
                .Include(h => h.HistoriaCompartida)
                    .ThenInclude(c => c.Archivos)
                .FirstOrDefaultAsync(h => h.Id == historiaClinicaMedicoId, ct);

            if (historia == null) throw new KeyNotFoundException("Historia no encontrada.");

            // Tomamos el último archivo cargado (puedes cambiar la lógica)
            var archivo = historia.HistoriaCompartida.Archivos
                .OrderByDescending(a => a.FechaSubida).FirstOrDefault();

            if (archivo == null) throw new InvalidOperationException("La historia no tiene archivos.");

            // archivo.RutaArchivo es relativa tipo /storage/... si seguiste el upload propuesto
            var physical = Path.Combine(_env.ContentRootPath, archivo.RutaArchivo.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar));
            var downloadName = archivo.NombreArchivo;
            var contentType = archivo.TipoArchivo.ToString().Equals("PDF", StringComparison.OrdinalIgnoreCase)
                ? "application/pdf" : "image/*";

            return (physical, contentType, downloadName);
        }
    }
}
