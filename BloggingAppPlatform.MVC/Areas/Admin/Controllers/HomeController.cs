using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.MVC.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggingAppPlatform.MVC.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOrModerator")]
[Area("Admin")]
public class HomeController(
    IUserRepository userRepo,
    IPostRepository postRepo,
    ICommentRepository commentRepo) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var users = await userRepo.GetAllWithRolesAsync(ct);
        var posts = await postRepo.GetAllAsync(ct);
        var comments = await commentRepo.GetAllAsync(ct);

        return View(new HomeVM
        {
            UserCount = users.Count,
            PostCount = posts.Count,
            CommentCount = comments.Count
        });
    }
}
