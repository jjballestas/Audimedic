namespace Audimedic_Backend.Services.Users
{
    using Audimedic_Backend.Data;
    public interface IHistoriaQueryService
    {
        
        Task<(IEnumerable<HistoriaClinicaMedico> items, int total)>
            GetMisHistoriasAsync(int medicoId, int page, int pageSize, string? estado, CancellationToken ct);
       
        
        Task<(string filePath, string contentType, string downloadName)>
            GetArchivoAsync(int historiaClinicaMedicoId, CancellationToken ct);
    }
}
