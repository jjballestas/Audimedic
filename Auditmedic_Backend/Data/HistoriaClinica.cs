namespace Audimedic_Backend.Data
{
    using Audimedic_Backend.Enums;
    public class HistoriaClinica
    {
        public int Id { get; set; }
        public string NumeroHistoria { get; set; } = null!;
        public string ArchivoUrl { get; set; } = null!;
        public DateTime FechaCarga { get; set; } = DateTime.UtcNow;

        public DateTime FechaCirugia { get; set; }
        public TimeSpan HoraCirugia { get; set; }
        public int TiempoQuirurgico { get; set; }
        public int TiempoAnestesico { get; set; }
        public string Sala { get; set; } = null!;

        public EstadoHistoria Estado { get; set; } = EstadoHistoria.Pendiente;

        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        public int? EntidadId { get; set; }   // <- antes era int
        public Entidad? Entidad { get; set; }

        public ICollection<ProcedimientoHistoria> Procedimientos { get; set; } = new List<ProcedimientoHistoria>();
    }
}