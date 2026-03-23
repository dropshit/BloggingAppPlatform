using BloggingApp.Application.Common.Interfaces;
using BloggingApp.Application.Common.Responses;
using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using ErrorOr;
using FluentValidation;

namespace BloggingApp.Application.Auth.Commands;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password,
    string RePassword);

public record RegisterResponse(string Token, DateTime Expiration);

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(30);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.RePassword).Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}

public static class RegisterHandler
{
    public static async Task<ErrorOr<RegisterResponse>> Handle(
        RegisterCommand command,
        IUserRepository userRepo,
        IUserOperationClaimRepository userClaimRepo,
        IHashingService hashing,
        IJwtService jwt,
        CancellationToken ct)
    {
        if (await userRepo.GetByEmailAsync(command.Email, ct) is not null)
            return Error.Conflict("Auth.EmailTaken", "Email is already in use");

        if (await userRepo.GetByUsernameAsync(command.Username, ct) is not null)
            return Error.Conflict("Auth.UsernameTaken", "Username is already in use");

        hashing.CreateHash(command.Password, out var hash, out var salt);

        var user = new User
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Username = command.Username,
            Email = command.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            JoinDate = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow,
            Status = true
        };

        await userRepo.AddAsync(user, ct);
        await userClaimRepo.AddAsync(new UserOperationClaim { UserId = user.Id, OperationClaimId = 1 }, ct);

        var claims = await userRepo.GetClaimsAsync(user.Id, ct);
        var token = jwt.CreateToken(user, claims);
        return new RegisterResponse(token.Token, token.Expiration);
    }
}
