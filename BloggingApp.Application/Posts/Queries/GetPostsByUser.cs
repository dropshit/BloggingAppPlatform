using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Posts.Queries;

public record GetPostsByUserQuery(int UserId);

public static class GetPostsByUserHandler
{
    public static async Task<ErrorOr<List<PostDetail>>> Handle(
        GetPostsByUserQuery query,
        IPostRepository repository,
        CancellationToken ct)
    {
        var posts = await repository.GetByUserIdWithDetailsAsync(query.UserId, ct);
        return posts;
    }
}
