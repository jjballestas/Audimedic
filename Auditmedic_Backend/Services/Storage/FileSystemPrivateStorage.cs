using Audimedic_Backend.Config;
using Microsoft.Extensions.Options;

namespace Audimedic_Backend.Services.Storage
{

    public class FileSystemPrivateStorage : IPrivateStorage
    {
        private readonly string _root;

        // Tomamos la ruta relativa del appsettings y la combinamos con ContentRootPath
        public FileSystemPrivateStorage(IOptions<StorageOptions> opts, IWebHostEnvironment env)
        {
            // RootPath puede venir como "App_Data/Storage"
            var configured = opts.Value.RootPath?.Trim() ?? "App_Data/Storage";
            _root = Path.Combine(env.ContentRootPath, configured);
            Directory.CreateDirectory(_root);
        }

        public async Task<string> SaveAsync(Stream file, string fileName, string relativeFolder, CancellationToken ct = default)
        {
            var safeFolder = Sanitize(relativeFolder);
            var folder = Path.Combine(_root, safeFolder);
            Directory.CreateDirectory(folder);

            var safeName = Path.GetFileName(fileName);
            var finalName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{safeName}";
            var physical = Path.Combine(folder, finalName);

            using (var fs = new FileStream(physical, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(fs, ct);
            }

            // Devolvemos solo la ruta relativa (para guardar en BD)
            var relative = Path.Combine(safeFolder, finalName).Replace('\\', '/');
            return relative;
        }

        public string GetPhysicalPath(string relativePath)
        {
            var safe = Sanitize(relativePath);
            return Path.Combine(_root, safe);
        }

        private static string Sanitize(string path)
        {
            var invalid = Path.GetInvalidPathChars();
            var cleaned = new string(path.Where(c => !invalid.Contains(c)).ToArray())
                .Replace("..", "")
                .TrimStart('\\', '/');
            return cleaned.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
