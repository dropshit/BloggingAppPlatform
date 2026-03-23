using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Infrastructure.Persistence.Repositories;

public class UserRepository(BloggingAppDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        => await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => await db.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<List<OperationClaim>> GetClaimsAsync(int userId, CancellationToken ct = default)
        => await db.UserOperationClaims
            .Where(uc => uc.UserId == userId)
            .Join(db.OperationClaims,
                uc => uc.OperationClaimId,
                oc => oc.Id,
                (_, oc) => oc)
            .ToListAsync(ct);

    public async Task<List<UserDetail>> GetAllWithRolesAsync(CancellationToken ct = default)
        => await db.Users
            .Select(u => new UserDetail(
                u.Id,
                u.FirstName,
                u.LastName,
                u.Username,
                u.Email,
                u.JoinDate,
                string.Join(", ", db.UserOperationClaims
                    .Where(uc => uc.UserId == u.Id)
                    .Join(db.OperationClaims,
                        uc => uc.OperationClaimId,
                        oc => oc.Id,
                        (_, oc) => oc.Name)
                    .ToList())))
            .ToListAsync(ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
    }
}
