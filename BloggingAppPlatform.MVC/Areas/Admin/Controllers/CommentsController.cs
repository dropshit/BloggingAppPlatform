using System.Security.Claims;
using BloggingApp.Application.Comments.Commands;
using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.MVC.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.MVC.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOrModerator")]
[Area("Admin")]
public class CommentsController(ICommentRepository commentRepo, IMessageBus bus) : Controller
{
    private int GetUserId() => int.Parse(User.FindFirstValue("userId")!);

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var comments = await commentRepo.GetAllWithDetailsAsync(ct);
        return View(new CommentVM { Comments = comments });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteComment(int commentId, CancellationToken ct)
    {
        var command = new DeleteCommentCommand(commentId, GetUserId(), true);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(command, ct);
        TempData[result.IsError ? "Error" : "Message"] =
            result.IsError ? result.FirstError.Description : "Comment deleted successfully.";
        return RedirectToAction("Index");
    }
}
