using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Posts.Queries;

public record GetAllPostsQuery(int Page = 1, int PageSize = 10);

public static class GetAllPostsHandler
{
    public static async Task<ErrorOr<List<PostDetail>>> Handle(
        GetAllPostsQuery query,
        IPostRepository repository,
        CancellationToken ct)
    {
        var posts = await repository.GetAllWithDetailsAsync(query.Page, query.PageSize, ct);
        return posts;
    }
}
