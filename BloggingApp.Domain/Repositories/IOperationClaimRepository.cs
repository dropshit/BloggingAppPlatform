using BloggingApp.Domain.Entities;

namespace BloggingApp.Domain.Repositories;

public interface IOperationClaimRepository
{
    Task<OperationClaim?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<List<OperationClaim>> GetAllAsync(CancellationToken ct = default);
}
