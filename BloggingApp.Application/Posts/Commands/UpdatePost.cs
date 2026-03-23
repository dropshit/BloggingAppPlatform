using BloggingApp.Domain.Repositories;
using ErrorOr;
using FluentValidation;

namespace BloggingApp.Application.Posts.Commands;

public record UpdatePostCommand(int PostId, int UserId, string Title, string Context);

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Context).NotEmpty().MaximumLength(1000);
    }
}

public static class UpdatePostHandler
{
    public static async Task<ErrorOr<Updated>> Handle(
        UpdatePostCommand command,
        IPostRepository repository,
        CancellationToken ct)
    {
        var post = await repository.GetByIdAsync(command.PostId, ct);

        if (post is null)
            return Error.NotFound("Post.NotFound", "Post not found");

        if (post.UserId != command.UserId)
            return Error.Forbidden("Post.Forbidden", "You do not have permission to update this post");

        post.Title = command.Title;
        post.Context = command.Context;
        post.UpdateTime = DateTime.UtcNow;

        await repository.UpdateAsync(post, ct);
        return Result.Updated;
    }
}
