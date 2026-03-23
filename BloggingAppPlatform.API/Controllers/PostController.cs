using BloggingApp.Application.Posts.Commands;
using BloggingApp.Application.Posts.Queries;
using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wolverine;

namespace BloggingAppPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController(IMessageBus bus) : ApiController
{
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private bool HasDeleteClaim() =>
        User.IsInRole("Admin") || User.HasClaim(ClaimTypes.Role, "post.delete");

    [Authorize]
    [HttpPost("addPost")]
    public async Task<IActionResult> AddPost(CreatePostBody body, CancellationToken ct)
    {
        var command = new CreatePostCommand(GetUserId(), body.Title, body.Context);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<PostCreatedResponse>>(command, ct);
        return result.Match(r => Ok(r), ErrorResult);
    }

    [Authorize]
    [HttpPut("updatePost")]
    public async Task<IActionResult> UpdatePost(UpdatePostBody body, CancellationToken ct)
    {
        var command = new UpdatePostCommand(body.PostId, GetUserId(), body.Title, body.Context);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Updated>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }

    [Authorize]
    [HttpDelete("deletePost")]
    public async Task<IActionResult> DeletePost(int postId, CancellationToken ct)
    {
        var command = new DeletePostCommand(postId, GetUserId(), HasDeleteClaim());
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }

    [HttpGet("getAllPosts")]
    public async Task<IActionResult> GetAllPosts(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        var query = new GetAllPostsQuery(page, pageSize);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<List<PostDetail>>>(query, ct);
        return result.Match(r => Ok(r), ErrorResult);
    }

    [HttpGet("getPostsByUserId")]
    public async Task<IActionResult> GetPostsByUserId(int userId, CancellationToken ct)
    {
        var query = new GetPostsByUserQuery(userId);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<List<PostDetail>>>(query, ct);
        return result.Match(r => Ok(r), ErrorResult);
    }
}

public record CreatePostBody(string Title, string Context);
public record UpdatePostBody(int PostId, string Title, string Context);
