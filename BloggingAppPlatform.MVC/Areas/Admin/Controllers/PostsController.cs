using System.Security.Claims;
using BloggingApp.Application.Posts.Commands;
using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.MVC.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.MVC.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOrModerator")]
[Area("Admin")]
public class PostsController(IPostRepository postRepo, IMessageBus bus) : Controller
{
    private int GetUserId() => int.Parse(User.FindFirstValue("userId")!);

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var posts = await postRepo.GetAllWithDetailsAsync(1, int.MaxValue, ct);
        return View(new PostVM { Posts = posts });
    }

    [HttpPost]
    public async Task<IActionResult> DeletePost(int postId, CancellationToken ct)
    {
        var command = new DeletePostCommand(postId, GetUserId(), true);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(command, ct);
        TempData[result.IsError ? "Error" : "Message"] =
            result.IsError ? result.FirstError.Description : "Post deleted successfully.";
        return RedirectToAction("Index");
    }
}
