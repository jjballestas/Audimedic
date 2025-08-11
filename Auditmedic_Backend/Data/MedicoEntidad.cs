using System.Diagnostics.Contracts;

namespace Audimedic_Backend.Data
{
    public class MedicoEntidad
    {
        public int Id { get; set; }

        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        public int EntidadId { get; set; }
        public Entidad Entidad { get; set; } = null!;

        // Opcional: datos contractuales por relación
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
    }
}
