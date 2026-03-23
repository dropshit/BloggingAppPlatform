using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Users.Commands;

public record AddOperationClaimCommand(string Username, string ClaimName);

public static class AddOperationClaimHandler
{
    public static async Task<ErrorOr<Success>> Handle(
        AddOperationClaimCommand command,
        IUserRepository userRepo,
        IOperationClaimRepository claimRepo,
        IUserOperationClaimRepository userClaimRepo,
        CancellationToken ct)
    {
        var user = await userRepo.GetByUsernameAsync(command.Username, ct);
        if (user is null)
            return Error.NotFound("User.NotFound", "User not found");

        var claim = await claimRepo.GetByNameAsync(command.ClaimName, ct);
        if (claim is null)
            return Error.NotFound("Claim.NotFound", "Operation claim not found");

        if (await userClaimRepo.ExistsAsync(user.Id, claim.Id, ct))
            return Error.Conflict("Claim.AlreadyAssigned", "User already has this claim");

        await userClaimRepo.AddAsync(new() { UserId = user.Id, OperationClaimId = claim.Id }, ct);
        return Result.Success;
    }
}
