using BloggingApp.Application.Common.Interfaces;
using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace BloggingApp.Application.Reports.Commands;

public record AddReportCommand(string ReportedBy, string ReportedUser, IFormFile Photo);

public static class AddReportHandler
{
    public static async Task<ErrorOr<Success>> Handle(
        AddReportCommand command,
        IReportRepository repository,
        IFileStorageService fileStorage,
        CancellationToken ct)
    {
        var photoUrl = fileStorage.SaveImage(command.Photo);

        await repository.AddAsync(new Report
        {
            ReporetedBy = command.ReportedBy,
            ReportedUser = command.ReportedUser,
            PhotoUrl = photoUrl,
            CreateDate = DateTime.UtcNow
        }, ct);

        return Result.Success;
    }
}
