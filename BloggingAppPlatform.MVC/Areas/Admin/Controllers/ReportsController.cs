using BloggingApp.Application.Reports.Commands;
using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.MVC.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.MVC.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOrModerator")]
[Area("Admin")]
public class ReportsController(IReportRepository reportRepo, IMessageBus bus) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var reports = await reportRepo.GetAllAsync(ct);
        return View(new ReportVM { Reports = reports });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteReport(int Id, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(new DeleteReportCommand(Id), ct);
        TempData[result.IsError ? "Error" : "Message"] =
            result.IsError ? result.FirstError.Description : "Report deleted successfully.";
        return RedirectToAction("Index");
    }
}
