using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Users.Commands;

public record FollowUserCommand(int FollowerId, int FollowedUserId);

public static class FollowUserHandler
{
    public static async Task<ErrorOr<Success>> Handle(
        FollowUserCommand command,
        IUserFollowerRepository repository,
        CancellationToken ct)
    {
        if (await repository.ExistsAsync(command.FollowerId, command.FollowedUserId, ct))
            return Error.Conflict("Follow.AlreadyFollowing", "You are already following this user");

        await repository.AddAsync(new UserFollower
        {
            FollowerId = command.FollowerId,
            FollowedUserId = command.FollowedUserId,
            CreateDate = DateTime.UtcNow
        }, ct);

        return Result.Success;
    }
}
