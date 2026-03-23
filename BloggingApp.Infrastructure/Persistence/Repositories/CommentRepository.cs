using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Infrastructure.Persistence.Repositories;

public class CommentRepository(BloggingAppDbContext db) : ICommentRepository
{
    public async Task<Comment?> GetByIdAsync(int id, CancellationToken ct = default)
        => await db.Comments.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct);

    public async Task<List<Comment>> GetByPostIdAsync(int postId, CancellationToken ct = default)
        => await db.Comments
            .Where(c => c.PostId == postId && !c.IsDeleted)
            .OrderBy(c => c.CreateDate)
            .ToListAsync(ct);

    public async Task<List<Comment>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await db.Comments
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .OrderByDescending(c => c.CreateDate)
            .ToListAsync(ct);

    public async Task<List<Comment>> GetAllAsync(CancellationToken ct = default)
        => await db.Comments
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreateDate)
            .ToListAsync(ct);

    public async Task<List<CommentDetail>> GetAllWithDetailsAsync(CancellationToken ct = default)
        => await db.Comments
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreateDate)
            .Join(db.Users,
                c => c.UserId,
                u => u.Id,
                (c, u) => new CommentDetail(
                    c.Id,
                    c.PostId,
                    c.UserId,
                    u.Username,
                    c.CommentText,
                    c.CreateDate))
            .ToListAsync(ct);

    public async Task<List<CommentDetail>> GetByPostIdWithDetailsAsync(int postId, CancellationToken ct = default)
        => await db.Comments
            .Where(c => c.PostId == postId && !c.IsDeleted)
            .OrderBy(c => c.CreateDate)
            .Join(db.Users,
                c => c.UserId,
                u => u.Id,
                (c, u) => new CommentDetail(
                    c.Id,
                    c.PostId,
                    c.UserId,
                    u.Username,
                    c.CommentText,
                    c.CreateDate))
            .ToListAsync(ct);

    public async Task AddAsync(Comment comment, CancellationToken ct = default)
    {
        await db.Comments.AddAsync(comment, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Comment comment, CancellationToken ct = default)
    {
        db.Comments.Update(comment);
        await db.SaveChangesAsync(ct);
    }
}
