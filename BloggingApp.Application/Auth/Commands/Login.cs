using BloggingApp.Application.Common.Interfaces;
using BloggingApp.Domain.Repositories;
using ErrorOr;
using FluentValidation;

namespace BloggingApp.Application.Auth.Commands;

public record LoginCommand(string Username, string Password);

public record LoginResponse(string Token, DateTime Expiration);

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public static class LoginHandler
{
    public static async Task<ErrorOr<LoginResponse>> Handle(
        LoginCommand command,
        IUserRepository userRepo,
        IHashingService hashing,
        IJwtService jwt,
        CancellationToken ct)
    {
        var user = await userRepo.GetByUsernameAsync(command.Username, ct);
        if (user is null)
            return Error.NotFound("Auth.UserNotFound", "User not found");

        if (!hashing.VerifyHash(command.Password, user.PasswordHash!, user.PasswordSalt!))
            return Error.Validation("Auth.InvalidCredentials", "Username or password is wrong");

        var claims = await userRepo.GetClaimsAsync(user.Id, ct);
        var token = jwt.CreateToken(user, claims);
        return new LoginResponse(token.Token, token.Expiration);
    }
}
