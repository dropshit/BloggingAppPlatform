using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Comments.Queries;

public record GetCommentsByUserQuery(int UserId);

public static class GetCommentsByUserHandler
{
    public static async Task<ErrorOr<List<Comment>>> Handle(
        GetCommentsByUserQuery query,
        ICommentRepository repository,
        CancellationToken ct)
    {
        var comments = await repository.GetByUserIdAsync(query.UserId, ct);
        return comments;
    }
}
