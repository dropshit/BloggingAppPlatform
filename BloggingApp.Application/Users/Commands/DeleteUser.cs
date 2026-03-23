using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Users.Commands;

public record DeleteUserCommand(int UserId);

public static class DeleteUserHandler
{
    public static async Task<ErrorOr<Deleted>> Handle(
        DeleteUserCommand command,
        IUserRepository repository,
        CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Error.NotFound("User.NotFound", "User not found");

        user.Status = false;
        await repository.UpdateAsync(user, ct);
        return Result.Deleted;
    }
}
