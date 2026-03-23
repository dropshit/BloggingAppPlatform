using BloggingApp.Application.Users.Commands;
using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.MVC.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.MVC.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOrModerator")]
[Area("Admin")]
public class UsersController(IUserRepository userRepo, IMessageBus bus) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var users = await userRepo.GetAllWithRolesAsync(ct);
        return View(new UserVM { Users = users });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int UserId, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(new DeleteUserCommand(UserId), ct);
        TempData[result.IsError ? "Error" : "Message"] =
            result.IsError ? result.FirstError.Description : "User deleted successfully.";
        return RedirectToAction("Index");
    }
}
