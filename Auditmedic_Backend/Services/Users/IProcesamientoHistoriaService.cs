namespace Audimedic_Backend.Services.Users
{
    public interface IProcesamientoHistoriaService
    {
        Task ProcesarAsync(int historiaClinicaMedicoId, CancellationToken ct);
    }
}
