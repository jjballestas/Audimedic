namespace Audimedic_Backend.Data
{
    public class TarifaSOAT
    {
        public int Id { get; set; }
        public int ProcedimientoId { get; set; }
        public Procedimiento Procedimiento { get; set; } = null!;

        public int Anio { get; set; }
        public decimal Valor { get; set; } // valor oficial por año
    }
}
