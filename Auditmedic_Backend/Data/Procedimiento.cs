namespace Audimedic_Backend.Data
{
    public class Procedimiento
    {
        public int Id { get; set; }
        public string CodigoCUPS { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public ICollection<TarifaContrato> TarifasContrato { get; set; } = new List<TarifaContrato>();
        public ICollection<TarifaSOAT> TarifasSOAT { get; set; } = new List<TarifaSOAT>();
    }
}
