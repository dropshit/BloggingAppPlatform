using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Comments.Queries;

public record GetCommentsByPostQuery(int PostId);

public static class GetCommentsByPostHandler
{
    public static async Task<ErrorOr<List<CommentDetail>>> Handle(
        GetCommentsByPostQuery query,
        ICommentRepository repository,
        CancellationToken ct)
    {
        var comments = await repository.GetByPostIdWithDetailsAsync(query.PostId, ct);
        return comments;
    }
}
