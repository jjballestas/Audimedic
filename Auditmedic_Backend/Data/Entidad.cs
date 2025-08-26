
using Audimedic_Backend.Enums;
using Microsoft.EntityFrameworkCore;
namespace Audimedic_Backend.Data
{
   

    [Index(nameof(Codigo), IsUnique = true)]
    public class Entidad
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = null!;
        public TipoEntidad TipoEntidad { get; set; }
        public ICollection<MedicoEntidad> Medicos { get; set; } = new List<MedicoEntidad>();
        public ICollection<HistoriaCompartida> HistoriasCompartidas { get; set; } = new List<HistoriaCompartida>();
        
    }
}
