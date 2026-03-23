using BloggingApp.Domain.Repositories;
using ErrorOr;
using FluentValidation;

namespace BloggingApp.Application.Comments.Commands;

public record UpdateCommentCommand(int CommentId, int UserId, string CommentText);

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.CommentText).NotEmpty().MaximumLength(500);
    }
}

public static class UpdateCommentHandler
{
    public static async Task<ErrorOr<Updated>> Handle(
        UpdateCommentCommand command,
        ICommentRepository repository,
        CancellationToken ct)
    {
        var comment = await repository.GetByIdAsync(command.CommentId, ct);

        if (comment is null)
            return Error.NotFound("Comment.NotFound", "Comment not found");

        if (comment.UserId != command.UserId)
            return Error.Forbidden("Comment.Forbidden", "You do not have permission to update this comment");

        comment.CommentText = command.CommentText;
        comment.UpdateTime = DateTime.UtcNow;
        comment.IsDeleted = false;

        await repository.UpdateAsync(comment, ct);
        return Result.Updated;
    }
}
