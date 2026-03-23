using BloggingApp.Domain.Entities;

namespace BloggingApp.Domain.Repositories;

public interface IUserFollowerRepository
{
    Task<UserFollower?> GetAsync(int followerId, int followedUserId, CancellationToken ct = default);
    Task<bool> ExistsAsync(int followerId, int followedUserId, CancellationToken ct = default);
    Task<int> GetFollowerCountAsync(int userId, CancellationToken ct = default);
    Task AddAsync(UserFollower follower, CancellationToken ct = default);
    Task DeleteAsync(UserFollower follower, CancellationToken ct = default);
}
