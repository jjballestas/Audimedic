namespace Audimedic_Backend.Data
{
    public class TarifaContrato
    {
        public int Id { get; set; }

        public int ContratoId { get; set; }
        public Contrato Contrato { get; set; } = null!;

        public int ProcedimientoId { get; set; }
        public Procedimiento Procedimiento { get; set; } = null!;

        // Si el contrato tiene tarifa especial para este procedimiento
        public decimal? ValorFijo { get; set; }      // si aplica
        public decimal? FactorPorcentaje { get; set; } // sobre base (SOAT/ISS), ej: 1.2 => +20%
    }
}
