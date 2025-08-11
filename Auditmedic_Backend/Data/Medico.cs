namespace Audimedic_Backend.Data
{
    public class Medico
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public ICollection<MedicoEntidad> Entidades { get; set; } = new List<MedicoEntidad>();
        //public ICollection<HistoriaClinica> Historias { get; set; } = new List<HistoriaClinica>();
        public ICollection<HistoriaClinicaMedico> Historias { get; set; } = new List<HistoriaClinicaMedico>();
    }
}
