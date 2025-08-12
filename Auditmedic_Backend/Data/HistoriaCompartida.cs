namespace Audimedic_Backend.Data
{
    public class HistoriaCompartida
    {
        public int Id { get; set; }
        public string NumeroHistoria { get; set; } = null!;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public int EntidadId { get; set; }
        public Entidad Entidad { get; set; } = null!;

        // Archivos físicos compartidos por todos los médicos
        public ICollection<ArchivoHistoriaClinica> Archivos { get; set; } = new List<ArchivoHistoriaClinica>();

        // Instancias por médico
        public ICollection<HistoriaClinicaMedico> HistoriasMedico { get; set; } = new List<HistoriaClinicaMedico>();
    }
}
