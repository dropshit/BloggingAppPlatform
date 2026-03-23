using BloggingApp.Application.Common.Interfaces;
using BloggingApp.Domain.Repositories;
using BloggingApp.Infrastructure.Auth;
using BloggingApp.Infrastructure.FileStorage;
using BloggingApp.Infrastructure.Persistence;
using BloggingApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BloggingApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<BloggingAppDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IUserFollowerRepository, UserFollowerRepository>();
        services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        services.AddScoped<IUserOperationClaimRepository, UserOperationClaimRepository>();

        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IHashingService, HashingService>();
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}
