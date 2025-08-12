namespace Audimedic_Backend.Services.Users
{
    using Audimedic_Backend.Data;
    using Microsoft.EntityFrameworkCore;
    public class ProcesamientoHistoriaService : IProcesamientoHistoriaService
    {
        private readonly AudimedicDbContext _db;

        public ProcesamientoHistoriaService(AudimedicDbContext db)
        {
            _db = db;
        }

        public async Task ProcesarAsync(int historiaClinicaMedicoId, CancellationToken ct)
        {
            // TODO: 1) leer archivo compartido, 2) extraer CUPS, 3) poblar ProcedimientoHistoria,
            // 4) aplicar reglas (contrato/tarifa SOAT/ISS), 5) calcular factura + líneas, 6) actualizar estado

            var historia = await _db.HistoriasClinicasMedico
                .Include(h => h.HistoriaCompartida).ThenInclude(c => c.Archivos)
                .FirstOrDefaultAsync(h => h.Id == historiaClinicaMedicoId, ct);

            if (historia == null) throw new KeyNotFoundException("Historia no encontrada.");

            // Por ahora, solo deja el marcador para continuar:
            // historia.Estado = EstadoHistoria.Calculada;
            // await _db.SaveChangesAsync(ct);
        }
    }
}
