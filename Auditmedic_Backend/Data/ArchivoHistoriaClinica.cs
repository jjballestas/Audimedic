using Audimedic_Backend.Enums;

namespace Audimedic_Backend.Data
{
    public class ArchivoHistoriaClinica
    {
        public int Id { get; set; }
        public int HistoriaCompartidaId { get; set; }
        public HistoriaCompartida HistoriaCompartida { get; set; } = null!;

        public string NombreArchivo { get; set; } = null!;
        public string RutaArchivo { get; set; } = null!; // Carpeta o bucket donde se almacena
        public TipoArchivoHistoria TipoArchivo { get; set; } = TipoArchivoHistoria.PDF; 

        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
    }
}
