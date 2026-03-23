using BloggingApp.Domain.Entities;

namespace BloggingApp.Domain.Repositories;

public interface IUserOperationClaimRepository
{
    Task<bool> ExistsAsync(int userId, int operationClaimId, CancellationToken ct = default);
    Task AddAsync(UserOperationClaim claim, CancellationToken ct = default);
}
