using BloggingApp.Domain.Repositories;
using ErrorOr;
using FluentValidation;

namespace BloggingApp.Application.Users.Commands;

public record UpdateUserCommand(int TargetUserId, int RequestingUserId, string Username, string Email, string FirstName, string LastName);

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(30);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
    }
}

public static class UpdateUserHandler
{
    public static async Task<ErrorOr<Updated>> Handle(
        UpdateUserCommand command,
        IUserRepository repository,
        CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(command.TargetUserId, ct);

        if (user is null || !user.Status)
            return Error.NotFound("User.NotFound", "User not found");

        if (user.Id != command.RequestingUserId)
            return Error.Forbidden("User.Forbidden", "You cannot update another user's profile");

        var emailExists = await repository.GetByEmailAsync(command.Email, ct);
        if (emailExists is not null && emailExists.Id != command.RequestingUserId)
            return Error.Conflict("User.EmailTaken", "Email is already in use by another user");

        var usernameExists = await repository.GetByUsernameAsync(command.Username, ct);
        if (usernameExists is not null && usernameExists.Id != command.RequestingUserId)
            return Error.Conflict("User.UsernameTaken", "Username is already in use by another user");

        user.Username = command.Username;
        user.Email = command.Email;
        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.UpdateTime = DateTime.UtcNow;

        await repository.UpdateAsync(user, ct);
        return Result.Updated;
    }
}
