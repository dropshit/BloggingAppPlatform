using BloggingApp.Domain.Entities;

namespace BloggingApp.Domain.Repositories;

// Projection type — lives in Domain as it's a read concern shared by any consumer
public record PostDetail(
    int PostId,
    int UserId,
    string Username,
    string Title,
    string Context,
    int CommentCount,
    string Date);

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Post>> GetAllAsync(CancellationToken ct = default);
    Task<List<Post>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<List<PostDetail>> GetAllWithDetailsAsync(int page, int pageSize, CancellationToken ct = default);
    Task<List<PostDetail>> GetByUserIdWithDetailsAsync(int userId, CancellationToken ct = default);
    Task AddAsync(Post post, CancellationToken ct = default);
    Task UpdateAsync(Post post, CancellationToken ct = default);
}
