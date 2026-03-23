using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Users.Queries;

public record GetAllUsersQuery;

// UserDetail is defined in Domain.Repositories

public static class GetAllUsersHandler
{
    public static async Task<ErrorOr<List<UserDetail>>> Handle(
        GetAllUsersQuery query,
        IUserRepository repository,
        CancellationToken ct)
    {
        var users = await repository.GetAllWithRolesAsync(ct);
        return users;
    }
}
