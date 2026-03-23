namespace BloggingApp.Domain.Entities;

public class Comment : BaseEntity
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public DateTime UpdateTime { get; set; }
}
