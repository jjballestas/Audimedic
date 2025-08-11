namespace Audimedic_Backend.Data
{
    public class FacturaLinea
    {
        public int Id { get; set; }

        public int FacturaId { get; set; }
        public Factura Factura { get; set; } = null!;

        // Referencia al procedimiento detectado en la historia
        public int ProcedimientoHistoriaId { get; set; }
        public ProcedimientoHistoria ProcedimientoHistoria { get; set; } = null!;

        public int Cantidad { get; set; } = 1;
        public decimal ValorUnitario { get; set; }   // valor base por unidad
        public decimal PorcentajeAplicado { get; set; } // ej. 1.00, 0.50, 0.25, 1.20
        public decimal Subtotal { get; set; }        // ValorUnitario * Cantidad * PorcentajeAplicado
    }
}
