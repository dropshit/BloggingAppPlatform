using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Reports.Queries;

public record GetAllReportsQuery;

public static class GetAllReportsHandler
{
    public static async Task<ErrorOr<List<Report>>> Handle(
        GetAllReportsQuery query,
        IReportRepository repository,
        CancellationToken ct)
    {
        var reports = await repository.GetAllAsync(ct);
        return reports;
    }
}
