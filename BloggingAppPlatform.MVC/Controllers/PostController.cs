using System.Security.Claims;
using BloggingApp.Application.Posts.Commands;
using BloggingAppPlatform.MVC.Models;
using BloggingAppPlatform.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.MVC.Controllers;

[Authorize]
public class PostController(IMessageBus bus) : Controller
{
    private int GetUserId() => int.Parse(User.FindFirstValue("userId")!);

    [HttpPost]
    public async Task<IActionResult> AddPost(AddPostForm form, CancellationToken ct)
    {
        var command = new CreatePostCommand(GetUserId(), form.Title, form.Context);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<PostCreatedResponse>>(command, ct);
        if (result.IsError)
        {
            TempData["Error"] = result.FirstError.Description;
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePost(UpdatePostForm form, CancellationToken ct)
    {
        var command = new UpdatePostCommand(form.PostId, GetUserId(), form.Title, form.Context);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Updated>>(command, ct);
        if (result.IsError)
        {
            ViewBag.ErrorMessage = result.FirstError.Description;
            return View(new UpdatePostVm { postDto = form });
        }
        TempData["SuccessMessage"] = "Post updated successfully!";
        return RedirectToAction("Index", "Profile");
    }

    [HttpGet]
    public IActionResult UpdatePostView(int PostId, string Title, string Context)
    {
        return View("UpdatePost", new UpdatePostVm
        {
            postDto = new UpdatePostForm { PostId = PostId, Title = Title, Context = Context }
        });
    }

    [HttpGet]
    public IActionResult Index() => View();
}
