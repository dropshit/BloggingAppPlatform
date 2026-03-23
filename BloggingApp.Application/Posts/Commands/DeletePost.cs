using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Posts.Commands;

public record DeletePostCommand(int PostId, int UserId, bool HasDeleteClaim);

public static class DeletePostHandler
{
    public static async Task<ErrorOr<Deleted>> Handle(
        DeletePostCommand command,
        IPostRepository repository,
        CancellationToken ct)
    {
        var post = await repository.GetByIdAsync(command.PostId, ct);

        if (post is null)
            return Error.NotFound("Post.NotFound", "Post not found");

        if (!command.HasDeleteClaim && post.UserId != command.UserId)
            return Error.Forbidden("Post.Forbidden", "You do not have permission to delete this post");

        post.IsDeleted = true;
        post.UpdateTime = DateTime.UtcNow;
        await repository.UpdateAsync(post, ct);
        return Result.Deleted;
    }
}
