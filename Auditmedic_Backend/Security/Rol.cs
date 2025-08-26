namespace Audimedic_Backend.Security
{
    public class Rol
    {
        public int Id { get; set; }
        public string NombreRol { get; set; } = null!;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
