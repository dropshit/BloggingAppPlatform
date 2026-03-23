using BloggingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Infrastructure.Persistence;

public class BloggingAppDbContext(DbContextOptions<BloggingAppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserFollower> UserFollowers { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<PostImage> PostImages { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OperationClaim>().HasData(
            new OperationClaim { Id = 1, Name = "User" },
            new OperationClaim { Id = 2, Name = "Admin" },
            new OperationClaim { Id = 3, Name = "Moderator" },
            new OperationClaim { Id = 4, Name = "post.delete" },
            new OperationClaim { Id = 5, Name = "comment.delete" },
            new OperationClaim { Id = 6, Name = "add.opclaim" },
            new OperationClaim { Id = 7, Name = "delete.report" }
        );

        modelBuilder.Entity<UserOperationClaim>().HasData(
            new UserOperationClaim { Id = 1, UserId = 1, OperationClaimId = 2 },
            new UserOperationClaim { Id = 2, UserId = 1, OperationClaimId = 3 },
            new UserOperationClaim { Id = 3, UserId = 1, OperationClaimId = 4 },
            new UserOperationClaim { Id = 4, UserId = 1, OperationClaimId = 5 },
            new UserOperationClaim { Id = 5, UserId = 1, OperationClaimId = 6 },
            new UserOperationClaim { Id = 6, UserId = 1, OperationClaimId = 7 }
        );
    }
}
