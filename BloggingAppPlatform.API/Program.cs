using BloggingApp.Application.Auth.Commands;
using BloggingApp.Infrastructure;
using BloggingApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BloggingApp.Infrastructure.Auth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Wolverine;
using Wolverine.FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure (DbContext, repositories, JWT, hashing)
builder.Services.AddInfrastructure(builder.Configuration);

// Wolverine CQRS
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(RegisterHandler).Assembly);
    opts.UseFluentValidation();
});

// JWT Auth
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>()
    ?? throw new InvalidOperationException("TokenOptions section missing from configuration.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),
            ValidateIssuer = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = tokenOptions.Audience,
            RequireExpirationTime = true
        };
    });

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("CanDeletePost", p =>
        p.RequireAssertion(ctx =>
            ctx.User.IsInRole("Admin") || ctx.User.HasClaim(ClaimTypes.Role, "post.delete")));
    opts.AddPolicy("CanDeleteComment", p =>
        p.RequireAssertion(ctx =>
            ctx.User.IsInRole("Admin") || ctx.User.HasClaim(ClaimTypes.Role, "comment.delete")));
    opts.AddPolicy("CanDeleteReport", p =>
        p.RequireRole("Admin", "Moderator"));
    opts.AddPolicy("CanAddOpClaim", p =>
        p.RequireAssertion(ctx =>
            ctx.User.IsInRole("Admin") || ctx.User.HasClaim(ClaimTypes.Role, "add.opclaim")));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "BloggingApp API",
        Description = ".NET 10 Clean Architecture"
    });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Apply pending migrations at startup (safe for containerized deployments)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BloggingAppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
