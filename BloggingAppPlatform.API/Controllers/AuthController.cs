using BloggingApp.Application.Auth.Commands;
using BloggingAppPlatform.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BloggingAppPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMessageBus bus) : ApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<RegisterResponse>>(command, ct);
        return result.Match(r => Ok(r), ErrorResult);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<ErrorOr.ErrorOr<LoginResponse>>(command, ct);
        return result.Match(r => Ok(r), ErrorResult);
    }
}
