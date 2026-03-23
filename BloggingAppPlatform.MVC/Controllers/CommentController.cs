using System.Security.Claims;
using BloggingApp.Application.Comments.Commands;
using BloggingAppPlatform.MVC.Models;
using BloggingAppPlatform.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.MVC.Controllers;

public class CommentController(IMessageBus bus) : Controller
{
    private int GetUserId() => int.Parse(User.FindFirstValue("userId")!);

    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> AddComment(AddCommentForm form, CancellationToken ct)
    {
        var command = new AddCommentCommand(GetUserId(), form.PostId, form.CommentText);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Success>>(command, ct);
        if (result.IsError)
            ModelState.AddModelError("", result.FirstError.Description);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteComment(int commentId, CancellationToken ct)
    {
        var command = new DeleteCommentCommand(commentId, GetUserId(), false);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(command, ct);
        if (result.IsError)
        {
            TempData["Error"] = result.FirstError.Description;
            return RedirectToAction("Error", "Home");
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateComment(UpdateCommentForm form, CancellationToken ct)
    {
        var command = new UpdateCommentCommand(form.CommentId, GetUserId(), form.CommentText);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Updated>>(command, ct);
        if (result.IsError)
        {
            ViewBag.ErrorMessage = result.FirstError.Description;
            return View(new UpdateCommentVM { CommentDto = form });
        }
        TempData["SuccessMessage"] = "Comment updated successfully!";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult UpdateCommentView(int CommentId, string CommentText)
    {
        return View("UpdateComment", new UpdateCommentVM
        {
            CommentDto = new UpdateCommentForm { CommentId = CommentId, CommentText = CommentText }
        });
    }
}
