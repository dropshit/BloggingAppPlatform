namespace BloggingApp.Domain.Entities;

public class PostImage : BaseEntity
{
    public int PostId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
