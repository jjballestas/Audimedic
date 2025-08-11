using Audimedic_Backend.Enums;

namespace Audimedic_Backend.Data
{
    public class Contrato
    {
        public int Id { get; set; }

        public int MedicoEntidadId { get; set; }
        public MedicoEntidad MedicoEntidad { get; set; } = null!;

        public ManualTarifario ManualTarifario { get; set; } = ManualTarifario.SOAT; // SOAT, ISS, Especial
        public decimal? PorcentajeAjuste { get; set; } // p.ej. +20%, -30% (usar 0.20, -0.30)
        public bool Activo { get; set; } = true;

        public ICollection<TarifaContrato> Tarifas { get; set; } = new List<TarifaContrato>();
    }
}
