using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.Misc;
using Services.CommonServices.Abstractions;

namespace Services.CommonServices.Implementations
{
    public class ImageService : IImageService
    {
        private readonly string _wwwRootPath;

        private const string RootFolder = "images";

        private readonly ILogger _logger;

        public ImageService(WWWRootPathHolder wwwRootPathHolder, ILogger<ImageService> logger)
        {
            _logger = logger;
            _wwwRootPath = wwwRootPathHolder.Path;
        }

        private void EnsureRootFolderExist()
        {
            var path = Path.Combine(_wwwRootPath, RootFolder);
            if (Directory.Exists(path)) return;
            _logger.LogWarning("Root Folder Is Missing, Creating");
            Directory.CreateDirectory(path);
        }

        private void EnsureSubFolderExist(string subfolder)
        {
            var path = Path.Combine(_wwwRootPath, RootFolder, subfolder);
            if (Directory.Exists(path)) return;
            _logger.LogWarning($"SubFolder \"{subfolder}\" Is Missing, Creating");
            Directory.CreateDirectory(path);
        }

        public async Task<string> Create(string filename, string folder, byte[] data)
        {
            EnsureRootFolderExist();
            EnsureSubFolderExist(folder);
            var guid = Guid.NewGuid().ToString("N");
            var guidFileName = guid + filename.Substring(filename.LastIndexOf('.'));
            var folderPath = Path.Combine(_wwwRootPath, RootFolder, folder);
            var filePath = Path.Combine(folderPath, guidFileName);

            await using var fileStream = new FileStream(filePath, FileMode.CreateNew);
            await fileStream.WriteAsync(data.AsMemory(0, data.Length));
            return guidFileName;
        }

        public Task<ICollection<string>> Enlist(string folder)
        {
            EnsureRootFolderExist();
            EnsureSubFolderExist(folder);
            var folderPath = Path.Combine(_wwwRootPath, RootFolder, folder);
            ICollection<string> files =
                Directory.GetFiles(folderPath)
                    .Select(p =>
                        p.Substring(p.LastIndexOf('\\') + 1)
                    )
                    .ToList();
            return Task.FromResult(files);
        }
    }
}