using System.Security.Claims;
using BloggingApp.Application.Users.Commands;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.MVC.Controllers;

public class FollowController(IMessageBus bus) : Controller
{
    private int GetUserId() => int.Parse(User.FindFirstValue("userId")!);

    [HttpPost]
    public async Task<IActionResult> FollowUser(int followedUserId, CancellationToken ct)
    {
        var command = new FollowUserCommand(GetUserId(), followedUserId);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Success>>(command, ct);
        if (result.IsError)
            TempData["Error"] = result.FirstError.Description;
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> UnfollowUser(int unfollowedUserId, CancellationToken ct)
    {
        var command = new UnfollowUserCommand(GetUserId(), unfollowedUserId);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(command, ct);
        if (result.IsError)
            TempData["Error"] = result.FirstError.Description;
        return RedirectToAction("Index", "Home");
    }
}
