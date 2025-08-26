using Audimedic_Backend.Data;
using static Audimedic_Backend.Data.AudimedicDbContext;

namespace Audimedic_Backend.Security
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ContrasenaHash { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;

        public ICollection<ProveedorExterno> ProveedoresExternos { get; set; } = new List<ProveedorExterno>();
    }
}
