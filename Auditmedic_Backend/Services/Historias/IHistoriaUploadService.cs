using Audimedic_Backend.Data;
using Audimedic_Backend.Enums;
using Audimedic_Backend.Models.Historias;
using Audimedic_Backend.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Audimedic_Backend.Services.Historias
{
    public interface IHistoriaUploadService
    {
        Task<(int historiaClinicaMedicoId, string rutaRelativa)> SubirAsync(
            IFormFile archivo,
            UploadHistoriaDto dto,
            CancellationToken ct = default);
    }
       
    
}