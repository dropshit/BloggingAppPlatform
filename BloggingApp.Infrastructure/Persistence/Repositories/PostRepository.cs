using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Infrastructure.Persistence.Repositories;

public class PostRepository(BloggingAppDbContext db) : IPostRepository
{
    public async Task<Post?> GetByIdAsync(int id, CancellationToken ct = default)
        => await db.Posts.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

    public async Task<List<Post>> GetAllAsync(CancellationToken ct = default)
        => await db.Posts
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.UpdateTime)
            .ToListAsync(ct);

    public async Task<List<Post>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await db.Posts
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .OrderByDescending(p => p.UpdateTime)
            .ToListAsync(ct);

    public async Task<List<PostDetail>> GetAllWithDetailsAsync(int page, int pageSize, CancellationToken ct = default)
        => await db.Posts
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.UpdateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostDetail(
                p.Id,
                p.UserId,
                db.Users.Where(u => u.Id == p.UserId).Select(u => u.Username).FirstOrDefault() ?? "",
                p.Title,
                p.Context,
                db.Comments.Count(c => c.PostId == p.Id && !c.IsDeleted),
                p.UpdateTime != null
                    ? p.UpdateTime.Value.ToString("H:mm | dd/MM/yyyy")
                    : p.CreateDate.ToString("H:mm | dd/MM/yyyy")))
            .ToListAsync(ct);

    public async Task<List<PostDetail>> GetByUserIdWithDetailsAsync(int userId, CancellationToken ct = default)
        => await db.Posts
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .OrderByDescending(p => p.UpdateTime)
            .Select(p => new PostDetail(
                p.Id,
                p.UserId,
                db.Users.Where(u => u.Id == p.UserId).Select(u => u.Username).FirstOrDefault() ?? "",
                p.Title,
                p.Context,
                db.Comments.Count(c => c.PostId == p.Id && !c.IsDeleted),
                p.UpdateTime != null
                    ? p.UpdateTime.Value.ToString("H:mm | dd/MM/yyyy")
                    : p.CreateDate.ToString("H:mm | dd/MM/yyyy")))
            .ToListAsync(ct);

    public async Task AddAsync(Post post, CancellationToken ct = default)
    {
        await db.Posts.AddAsync(post, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Post post, CancellationToken ct = default)
    {
        db.Posts.Update(post);
        await db.SaveChangesAsync(ct);
    }
}
