using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using ErrorOr;
using FluentValidation;

namespace BloggingApp.Application.Posts.Commands;

public record CreatePostCommand(int UserId, string Title, string Context);

public record PostCreatedResponse(int PostId, string Title, string Context);

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(40)
            .WithMessage("Title cannot be empty or exceed 40 characters");
        RuleFor(x => x.Context).NotEmpty().MaximumLength(1000)
            .WithMessage("Context cannot be empty or exceed 1000 characters");
    }
}

public static class CreatePostHandler
{
    public static async Task<ErrorOr<PostCreatedResponse>> Handle(
        CreatePostCommand command,
        IPostRepository repository,
        CancellationToken ct)
    {
        var post = new Post
        {
            UserId = command.UserId,
            Title = command.Title,
            Context = command.Context,
            CreateDate = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        await repository.AddAsync(post, ct);
        return new PostCreatedResponse(post.Id, post.Title, post.Context);
    }
}
