using BloggingApp.Application.Auth.Commands;
using BloggingApp.Application.Common.Interfaces;
using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using BloggingAppPlatform.MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggingAppPlatform.MVC.Controllers;

public class AuthController(
    IUserRepository userRepo,
    IHashingService hashing,
    IJwtService jwt) : Controller
{
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterForm form, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(form);

        if (form.Password != form.RePassword)
        {
            ViewBag.ErrorMessage = "Passwords do not match.";
            return View(form);
        }

        if (await userRepo.GetByEmailAsync(form.Email, ct) is not null)
        {
            ViewBag.ErrorMessage = "Email is already in use.";
            return View(form);
        }

        if (await userRepo.GetByUsernameAsync(form.Username, ct) is not null)
        {
            ViewBag.ErrorMessage = "Username is already in use.";
            return View(form);
        }

        hashing.CreateHash(form.Password, out var hash, out var salt);

        var user = new User
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Username = form.Username,
            Email = form.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            JoinDate = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow,
            Status = true
        };

        await userRepo.AddAsync(user, ct);

        TempData["SuccessMessage"] = "Registration successful! You can now log in.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginForm form, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = "Please provide valid credentials.";
            return View(form);
        }

        var user = await userRepo.GetByUsernameAsync(form.Username, ct);
        if (user is null || !hashing.VerifyHash(form.Password, user.PasswordHash!, user.PasswordSalt!))
        {
            ViewBag.ErrorMessage = "Username or password is incorrect.";
            return View(form);
        }

        var claims = await userRepo.GetClaimsAsync(user.Id, ct);
        var token = jwt.CreateToken(user, claims);

        Response.Cookies.Append("auth_token", token.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = token.Expiration
        });

        var cookieClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new("userId", user.Id.ToString()),
        };
        foreach (var c in claims)
            cookieClaims.Add(new(ClaimTypes.Role, c.Name));

        var identity = new ClaimsIdentity(cookieClaims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete("auth_token");
        return RedirectToAction("Index", "Home");
    }
}
