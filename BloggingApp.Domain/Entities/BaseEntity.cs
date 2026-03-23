namespace BloggingApp.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
