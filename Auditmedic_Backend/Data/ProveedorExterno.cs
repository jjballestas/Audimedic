namespace Audimedic_Backend.Data
{
    public class ProveedorExterno
    {
        public int Id { get; set; }
        public string Proveedor { get; set; } = null!; // Ej: "google"
        public string IdExterno { get; set; } = null!;
        public bool EmailConfirmado { get; set; } = true;
        public string? FotoUrl { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
