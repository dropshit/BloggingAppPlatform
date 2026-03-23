using BloggingApp.Domain.Repositories;

namespace BloggingAppPlatform.MVC.Areas.Admin.ViewModels;

public class CommentVM
{
    public List<CommentDetail> Comments { get; set; } = [];
}
