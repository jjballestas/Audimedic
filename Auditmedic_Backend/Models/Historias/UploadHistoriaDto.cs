namespace Audimedic_Backend.Models.Historias
{
    public class UploadHistoriaDto
    {
        public string CodigoEntidad { get; set; } = null!;
        public string NumeroHistoria { get; set; } = null!;
        public int MedicoId { get; set; }
        public DateTime FechaCirugia { get; set; }
        public TimeSpan HoraCirugia { get; set; }
        public int TiempoQuirurgico { get; set; }
        public int TiempoAnestesico { get; set; }
        public string Sala { get; set; } = null!;

         

    }
}
