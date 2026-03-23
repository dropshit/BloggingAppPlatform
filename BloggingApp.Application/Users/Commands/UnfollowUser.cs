using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Users.Commands;

public record UnfollowUserCommand(int FollowerId, int FollowedUserId);

public static class UnfollowUserHandler
{
    public static async Task<ErrorOr<Deleted>> Handle(
        UnfollowUserCommand command,
        IUserFollowerRepository repository,
        CancellationToken ct)
    {
        var follow = await repository.GetAsync(command.FollowerId, command.FollowedUserId, ct);
        if (follow is null)
            return Error.NotFound("Follow.NotFollowing", "You are not following this user");

        await repository.DeleteAsync(follow, ct);
        return Result.Deleted;
    }
}
