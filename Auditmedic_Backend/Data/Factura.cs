using Audimedic_Backend.Enums;

namespace Audimedic_Backend.Data
{
    public class Factura
    {
        public int Id { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
        public FacturaEstado Estado { get; set; } = FacturaEstado.Pendiente;
        public decimal Total { get; set; }
        public string? ArchivoPdf { get; set; }

        public int HistoriaClinicaMedicoId { get; set; }
        public HistoriaClinicaMedico HistoriaClinicaMedico { get; set; } = null!;

        public ICollection<FacturaLinea> Lineas { get; set; } = new List<FacturaLinea>();
    }
}
