using BloggingApp.Domain.Entities;

namespace BloggingApp.Domain.Repositories;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Report>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Report report, CancellationToken ct = default);
    Task UpdateAsync(Report report, CancellationToken ct = default);
}
