using BloggingApp.Application.Reports.Commands;
using BloggingApp.Application.Reports.Queries;
using BloggingApp.Domain.Entities;
using BloggingAppPlatform.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wolverine;

namespace BloggingAppPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController(IMessageBus bus) : ApiController
{
    private string GetUsername() => User.FindFirstValue(ClaimTypes.Name) ?? "";

    [Authorize]
    [HttpPost("addReport")]
    public async Task<IActionResult> AddReport([FromForm] AddReportBody body, CancellationToken ct)
    {
        var command = new AddReportCommand(GetUsername(), body.ReportedUser, body.Photo);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Success>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }

    [Authorize(Policy = "CanDeleteReport")]
    [HttpDelete("deleteReport")]
    public async Task<IActionResult> DeleteReport(int reportId, CancellationToken ct)
    {
        var command = new DeleteReportCommand(reportId);
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<ErrorOr.Deleted>>(command, ct);
        return result.Match(_ => Ok(), ErrorResult);
    }

    [Authorize(Policy = "CanDeleteReport")]
    [HttpGet("getAllReports")]
    public async Task<IActionResult> GetAllReports(CancellationToken ct)
    {
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<List<Report>>>(new GetAllReportsQuery(), ct);
        return result.Match(r => Ok(r), ErrorResult);
    }
}

public record AddReportBody(string ReportedUser, IFormFile Photo);
