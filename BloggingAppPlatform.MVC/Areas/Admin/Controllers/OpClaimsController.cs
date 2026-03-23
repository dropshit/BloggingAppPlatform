using BloggingApp.Application.Users.Commands;
using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.MVC.Areas.Admin.ViewModels;
using BloggingAppPlatform.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.MVC.Areas.Admin.Controllers;

[Area("Admin")]
public class OpClaimsController(IOperationClaimRepository claimRepo, IMessageBus bus) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var claims = await claimRepo.GetAllAsync(ct);
        return View(new OpClaimVM { OperationClaims = claims });
    }

    [HttpPost]
    public async Task<IActionResult> AddOpClaim(OpClaimForm form, CancellationToken ct)
    {
        var command = new AddOperationClaimCommand(form.Username, form.ClaimName);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Success>>(command, ct);
        if (result.IsError)
            ViewBag.ErrorMessage = result.FirstError.Description;
        else
            ViewBag.SuccessMessage = "Operation claim added successfully.";
        return RedirectToAction("Index");
    }
}
