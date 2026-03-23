using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Infrastructure.Persistence.Repositories;

public class UserFollowerRepository(BloggingAppDbContext db) : IUserFollowerRepository
{
    public async Task<UserFollower?> GetAsync(int followerId, int followedUserId, CancellationToken ct = default)
        => await db.UserFollowers.FirstOrDefaultAsync(
            f => f.FollowerId == followerId && f.FollowedUserId == followedUserId, ct);

    public async Task<bool> ExistsAsync(int followerId, int followedUserId, CancellationToken ct = default)
        => await db.UserFollowers.AnyAsync(
            f => f.FollowerId == followerId && f.FollowedUserId == followedUserId, ct);

    public async Task<int> GetFollowerCountAsync(int userId, CancellationToken ct = default)
        => await db.UserFollowers.CountAsync(f => f.FollowedUserId == userId, ct);

    public async Task AddAsync(UserFollower follower, CancellationToken ct = default)
    {
        await db.UserFollowers.AddAsync(follower, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(UserFollower follower, CancellationToken ct = default)
    {
        db.UserFollowers.Remove(follower);
        await db.SaveChangesAsync(ct);
    }
}
