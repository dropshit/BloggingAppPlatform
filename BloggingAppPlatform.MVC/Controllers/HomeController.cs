using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.MVC.Models;
using BloggingAppPlatform.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BloggingAppPlatform.MVC.Controllers;

public class HomeController(
    IPostRepository postRepo,
    ICommentRepository commentRepo) : Controller
{
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        int pageSize = 3;

        var posts = await postRepo.GetAllWithDetailsAsync(page, pageSize, ct);
        var allPosts = await postRepo.GetAllAsync(ct);
        int totalPosts = allPosts.Count;
        int totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

        var allComments = new List<BloggingApp.Domain.Repositories.CommentDetail>();
        foreach (var post in posts)
        {
            var comments = await commentRepo.GetByPostIdWithDetailsAsync(post.PostId, ct);
            allComments.AddRange(comments);
        }

        var viewModel = new PostVM
        {
            Posts = posts,
            Comments = allComments,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View(viewModel);
    }

    public IActionResult Error(int code)
    {
        return code == 404 ? View("NotFound") : View("Error");
    }
}
