using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Reports.Commands;

public record DeleteReportCommand(int ReportId);

public static class DeleteReportHandler
{
    public static async Task<ErrorOr<Deleted>> Handle(
        DeleteReportCommand command,
        IReportRepository repository,
        CancellationToken ct)
    {
        var report = await repository.GetByIdAsync(command.ReportId, ct);

        if (report is null)
            return Error.NotFound("Report.NotFound", "Report not found");

        report.IsDeleted = true;
        await repository.UpdateAsync(report, ct);
        return Result.Deleted;
    }
}
