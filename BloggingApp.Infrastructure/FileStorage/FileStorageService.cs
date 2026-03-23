using BloggingApp.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BloggingApp.Infrastructure.FileStorage;

public class FileStorageService(IWebHostEnvironment env) : IFileStorageService
{
    public string SaveImage(IFormFile file)
    {
        var fileName = Guid.NewGuid().ToString();
        var imagesFolder = Path.Combine(env.WebRootPath, "images");

        if (!Directory.Exists(imagesFolder))
            Directory.CreateDirectory(imagesFolder);

        var filePath = Path.Combine(imagesFolder, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);

        return fileName;
    }
}
