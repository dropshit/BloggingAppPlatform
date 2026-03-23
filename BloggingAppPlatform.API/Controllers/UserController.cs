using BloggingApp.Application.Users.Commands;
using BloggingApp.Application.Users.Queries;
using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wolverine;

namespace BloggingAppPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IMessageBus bus) : ApiController
{
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [Authorize(Policy = "CanAddOpClaim")]
    [HttpPost("addOperationClaim")]
    public async Task<IActionResult> AddOperationClaim(AddOperationClaimCommand command, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Success>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }

    [Authorize]
    [HttpPut("updateUser")]
    public async Task<IActionResult> UpdateUser(UpdateUserBody body, CancellationToken ct)
    {
        var userId = GetUserId();
        var command = new UpdateUserCommand(userId, userId, body.Username, body.Email, body.FirstName, body.LastName);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Updated>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }

    [HttpGet("getUsers")]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<List<UserDetail>>>(new GetAllUsersQuery(), ct);
        return result.Match(r => Ok(r), ErrorResult);
    }

    [Authorize]
    [HttpPost("follow")]
    public async Task<IActionResult> Follow(int followedUserId, CancellationToken ct)
    {
        var command = new FollowUserCommand(GetUserId(), followedUserId);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Success>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }

    [Authorize]
    [HttpDelete("unfollow")]
    public async Task<IActionResult> Unfollow(int followedUserId, CancellationToken ct)
    {
        var command = new UnfollowUserCommand(GetUserId(), followedUserId);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }
}

public record UpdateUserBody(string Username, string Email, string FirstName, string LastName);
