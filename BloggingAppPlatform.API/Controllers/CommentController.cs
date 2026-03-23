using BloggingApp.Application.Comments.Commands;
using BloggingApp.Application.Comments.Queries;
using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wolverine;

namespace BloggingAppPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController(IMessageBus bus) : ApiController
{
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private bool HasDeleteClaim() =>
        User.IsInRole("Admin") || User.HasClaim(ClaimTypes.Role, "comment.delete");

    [Authorize]
    [HttpPost("addComment")]
    public async Task<IActionResult> AddComment(AddCommentBody body, CancellationToken ct)
    {
        var command = new AddCommentCommand(GetUserId(), body.PostId, body.CommentText);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Success>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }

    [Authorize]
    [HttpPut("updateComment")]
    public async Task<IActionResult> UpdateComment(UpdateCommentBody body, CancellationToken ct)
    {
        var command = new UpdateCommentCommand(body.CommentId, GetUserId(), body.CommentText);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Updated>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }

    [Authorize]
    [HttpDelete("deleteComment")]
    public async Task<IActionResult> DeleteComment(int commentId, CancellationToken ct)
    {
        var command = new DeleteCommentCommand(commentId, GetUserId(), HasDeleteClaim());
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }

    [HttpGet("getCommentsByPostId")]
    public async Task<IActionResult> GetCommentsByPostId(int postId, CancellationToken ct)
    {
        var query = new GetCommentsByPostQuery(postId);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<List<CommentDetail>>>(query, ct);
        return result.Match(r => Ok(r), ErrorResult);
    }

    [HttpGet("getCommentsByUserId")]
    public async Task<IActionResult> GetCommentsByUserId(int userId, CancellationToken ct)
    {
        var query = new GetCommentsByUserQuery(userId);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<List<Comment>>>(query, ct);
        return result.Match(r => Ok(r), ErrorResult);
    }
}

public record AddCommentBody(int PostId, string CommentText);
public record UpdateCommentBody(int CommentId, string CommentText);
