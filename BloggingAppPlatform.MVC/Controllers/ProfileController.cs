using System.Security.Claims;
using BloggingApp.Application.Posts.Commands;
using BloggingApp.Application.Users.Commands;
using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.MVC.Models;
using BloggingAppPlatform.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.MVC.Controllers;

public class ProfileController(
    IPostRepository postRepo,
    IUserRepository userRepo,
    ICommentRepository commentRepo,
    IUserFollowerRepository followerRepo,
    IMessageBus bus) : Controller
{
    private int GetUserId() => int.Parse(User.FindFirstValue("userId")!);

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        int userId = GetUserId();
        var user = await userRepo.GetByIdAsync(userId, ct);
        var postsByUser = await postRepo.GetByUserIdWithDetailsAsync(userId, ct);
        var comments = await commentRepo.GetByUserIdAsync(userId, ct);
        int followerCount = await followerRepo.GetFollowerCountAsync(userId, ct);

        return View(new PostVM
        {
            PostsByUser = postsByUser,
            user = user,
            Count = postsByUser.Count,
            CommentCount = comments.Count,
            FollowerCount = followerCount
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile(string userName, CancellationToken ct)
    {
        int currentUserId = GetUserId();
        var user = await userRepo.GetByUsernameAsync(userName, ct);
        if (user is null)
            return RedirectToAction("Error", "Home");

        var postsByUser = await postRepo.GetByUserIdWithDetailsAsync(user.Id, ct);
        int followerCount = await followerRepo.GetFollowerCountAsync(user.Id, ct);
        bool isFollow = await followerRepo.ExistsAsync(currentUserId, user.Id, ct);

        return View("UserProfile", new PostVM
        {
            PostsByUser = postsByUser,
            user = user,
            Count = postsByUser.Count,
            IsFollow = isFollow,
            FollowerCount = followerCount
        });
    }

    [HttpPost]
    public async Task<IActionResult> DeletePost(int postId, CancellationToken ct)
    {
        var command = new DeletePostCommand(postId, GetUserId(), false);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(command, ct);
        if (result.IsError)
        {
            TempData["Error"] = result.FirstError.Description;
            return RedirectToAction("Error", "Home");
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult UpdateProfileView(int userId, string username, string firstname, string lastname, string email)
    {
        return View("UpdateProfile", new UpdateUserVm
        {
            User = new UpdateUserForm
            {
                Username = username,
                Firstname = firstname,
                Lastname = lastname,
                Email = email
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(UpdateUserForm form, CancellationToken ct)
    {
        int userId = GetUserId();
        var command = new UpdateUserCommand(userId, userId, form.Username, form.Email, form.Firstname, form.Lastname);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Updated>>(command, ct);
        if (result.IsError)
            ModelState.AddModelError("", result.FirstError.Description);
        return RedirectToAction("Index");
    }
}
