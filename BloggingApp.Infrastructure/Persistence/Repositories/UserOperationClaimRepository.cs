using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Infrastructure.Persistence.Repositories;

public class UserOperationClaimRepository(BloggingAppDbContext db) : IUserOperationClaimRepository
{
    public async Task<bool> ExistsAsync(int userId, int operationClaimId, CancellationToken ct = default)
        => await db.UserOperationClaims.AnyAsync(
            uc => uc.UserId == userId && uc.OperationClaimId == operationClaimId, ct);

    public async Task AddAsync(UserOperationClaim claim, CancellationToken ct = default)
    {
        await db.UserOperationClaims.AddAsync(claim, ct);
        await db.SaveChangesAsync(ct);
    }
}
