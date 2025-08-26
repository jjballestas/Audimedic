using Audimedic_Backend.Enums;

namespace Audimedic_Backend.Models.Entidades
{
    public record EntidadDto
    {
        public int Id { get; init; }
        public string Codigo { get; init; } = "";
        public string Nombre { get; init; } = "";
        public TipoEntidad TipoEntidad { get; init; }
    }

}
