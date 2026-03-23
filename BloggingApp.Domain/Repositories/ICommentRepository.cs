using BloggingApp.Domain.Entities;

namespace BloggingApp.Domain.Repositories;

public record CommentDetail(
    int CommentId,
    int PostId,
    int UserId,
    string Username,
    string CommentText,
    DateTime CommentTime);

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Comment>> GetByPostIdAsync(int postId, CancellationToken ct = default);
    Task<List<Comment>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<List<CommentDetail>> GetByPostIdWithDetailsAsync(int postId, CancellationToken ct = default);
    Task<List<Comment>> GetAllAsync(CancellationToken ct = default);
    Task<List<CommentDetail>> GetAllWithDetailsAsync(CancellationToken ct = default);
    Task AddAsync(Comment comment, CancellationToken ct = default);
    Task UpdateAsync(Comment comment, CancellationToken ct = default);
}
