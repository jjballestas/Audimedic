namespace Audimedic_Backend.Models.Historias
{
    public class HistoriaUploadRequest
    {
        public int MedicoId { get; set; }
        public int EntidadId { get; set; }
        public string NumeroHistoriaClinica { get; set; } = string.Empty;
        public IFormFile Archivo { get; set; } = default!;
    }
}
