using BloggingApp.Domain.Entities;

namespace BloggingApp.Domain.Repositories;

public record UserDetail(int Id, string FirstName, string LastName, string Username, string Email, DateTime JoinDate, string Roles);

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<List<OperationClaim>> GetClaimsAsync(int userId, CancellationToken ct = default);
    Task<List<UserDetail>> GetAllWithRolesAsync(CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
}
