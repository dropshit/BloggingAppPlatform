using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using ErrorOr;
using FluentValidation;

namespace BloggingApp.Application.Comments.Commands;

public record AddCommentCommand(int PostId, int UserId, string CommentText);

public record CommentCreatedResponse(int CommentId, int PostId, string CommentText);

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.CommentText).NotEmpty().MaximumLength(500)
            .WithMessage("Comment cannot be empty or exceed 500 characters");
    }
}

public static class AddCommentHandler
{
    public static async Task<ErrorOr<CommentCreatedResponse>> Handle(
        AddCommentCommand command,
        ICommentRepository repository,
        CancellationToken ct)
    {
        var comment = new Comment
        {
            PostId = command.PostId,
            UserId = command.UserId,
            CommentText = command.CommentText,
            CreateDate = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        await repository.AddAsync(comment, ct);
        return new CommentCreatedResponse(comment.Id, comment.PostId, comment.CommentText);
    }
}
