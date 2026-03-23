using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Infrastructure.Persistence.Repositories;

public class ReportRepository(BloggingAppDbContext db) : IReportRepository
{
    public async Task<Report?> GetByIdAsync(int id, CancellationToken ct = default)
        => await db.Reports.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<List<Report>> GetAllAsync(CancellationToken ct = default)
        => await db.Reports
            .OrderByDescending(r => r.CreateDate)
            .ToListAsync(ct);

    public async Task AddAsync(Report report, CancellationToken ct = default)
    {
        await db.Reports.AddAsync(report, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Report report, CancellationToken ct = default)
    {
        db.Reports.Update(report);
        await db.SaveChangesAsync(ct);
    }
}
