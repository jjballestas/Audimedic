using Audimedic_Backend.Enums;

namespace Audimedic_Backend.Data
{
    public class ProcedimientoHistoria
    {
        public int Id { get; set; }

        public int HistoriaClinicaMedicoId { get; set; }
        public HistoriaClinicaMedico HistoriaClinicaMedico { get; set; } = null!;

        public int ProcedimientoId { get; set; }
        public Procedimiento Procedimiento { get; set; } = null!;

        // Datos para liquidación:
        public bool EsPrincipal { get; set; } = false;   // determina regla % por vía
        public bool EsInherente { get; set; } = false;   // si es inherente, puede no liquidarse
        public bool Bilateral { get; set; } = false;     // si aplica bilateralidad

        public ViaQuirurgica ViaQuirurgica { get; set; } = ViaQuirurgica.Misma;
        public decimal? ValorCalculado { get; set; } // resultado de liquidación (persistimos para auditoría)
    }
}
