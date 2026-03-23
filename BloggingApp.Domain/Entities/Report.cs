namespace BloggingApp.Domain.Entities;

public class Report : BaseEntity
{
    public string ReporetedBy { get; set; } = string.Empty;
    public string ReportedUser { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
}
