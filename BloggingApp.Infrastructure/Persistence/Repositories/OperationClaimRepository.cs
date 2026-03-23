using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Infrastructure.Persistence.Repositories;

public class OperationClaimRepository(BloggingAppDbContext db) : IOperationClaimRepository
{
    public async Task<OperationClaim?> GetByNameAsync(string name, CancellationToken ct = default)
        => await db.OperationClaims.FirstOrDefaultAsync(oc => oc.Name == name, ct);

    public async Task<List<OperationClaim>> GetAllAsync(CancellationToken ct = default)
        => await db.OperationClaims.ToListAsync(ct);
}
