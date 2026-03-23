using Microsoft.AspNetCore.Http;

namespace BloggingApp.Application.Common.Interfaces;

public interface IFileStorageService
{
    string SaveImage(IFormFile file);
}
