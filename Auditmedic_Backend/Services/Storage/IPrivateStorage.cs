
    using System.Threading;
    using System.Threading.Tasks;

    namespace Audimedic_Backend.Services.Storage
    {
    public interface IPrivateStorage
    {
        Task<string> SaveAsync(Stream file, string fileName, string relativeFolder, CancellationToken ct = default);
        string GetPhysicalPath(string relativePath);
    }
}
 
