namespace BloggingApp.Domain.Entities;

public class Post : BaseEntity
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public DateTime? UpdateTime { get; set; }
}
