using BloggingApp.Domain.Repositories;

namespace BloggingAppPlatform.MVC.Areas.Admin.ViewModels;

public class PostVM
{
    public List<PostDetail> Posts { get; set; } = [];
}
