using BloggingApp.Domain.Repositories;
using ErrorOr;

namespace BloggingApp.Application.Comments.Commands;

public record DeleteCommentCommand(int CommentId, int UserId, bool HasDeleteClaim);

public static class DeleteCommentHandler
{
    public static async Task<ErrorOr<Deleted>> Handle(
        DeleteCommentCommand command,
        ICommentRepository repository,
        CancellationToken ct)
    {
        var comment = await repository.GetByIdAsync(command.CommentId, ct);

        if (comment is null)
            return Error.NotFound("Comment.NotFound", "Comment not found");

        if (!command.HasDeleteClaim && comment.UserId != command.UserId)
            return Error.Forbidden("Comment.Forbidden", "You do not have permission to delete this comment");

        comment.IsDeleted = true;
        comment.UpdateTime = DateTime.UtcNow;
        await repository.UpdateAsync(comment, ct);
        return Result.Deleted;
    }
}
