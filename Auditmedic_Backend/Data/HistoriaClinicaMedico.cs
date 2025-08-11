using Audimedic_Backend.Enums;

namespace Audimedic_Backend.Data
{
    public class HistoriaClinicaMedico
    {
        public int Id { get; set; }
        public DateTime FechaCarga { get; set; } = DateTime.UtcNow;

        public EstadoHistoria Estado { get; set; } = EstadoHistoria.Pendiente;

        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        public int HistoriaCompartidaId { get; set; }
        public HistoriaCompartida HistoriaCompartida { get; set; } = null!;

        // Campos propios de la intervención del médico
        public DateTime FechaCirugia { get; set; }
        public TimeSpan HoraCirugia { get; set; }
        public int TiempoQuirurgico { get; set; }
        public int TiempoAnestesico { get; set; }
        public string Sala { get; set; } = null!;

        public ICollection<ProcedimientoHistoria> Procedimientos { get; set; } = new List<ProcedimientoHistoria>();
        public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
    }
}
