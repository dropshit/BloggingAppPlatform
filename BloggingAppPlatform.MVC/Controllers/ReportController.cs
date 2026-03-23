using System.Security.Claims;
using BloggingApp.Application.Reports.Commands;
using BloggingAppPlatform.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.MVC.Controllers;

public class ReportController(IMessageBus bus) : Controller
{
    private string GetUsername() => User.FindFirstValue(System.Security.Claims.ClaimTypes.Name) ?? "";

    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Report([FromForm] ReportForm form, CancellationToken ct)
    {
        var command = new AddReportCommand(GetUsername(), form.ReportedUser, form.Photo);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Success>>(command, ct);
        if (result.IsError)
            TempData["Error"] = result.FirstError.Description;
        return RedirectToAction("Index", "Home");
    }
}

public class ReportForm
{
    public string ReportedUser { get; set; } = string.Empty;
    public Microsoft.AspNetCore.Http.IFormFile Photo { get; set; } = null!;
}
