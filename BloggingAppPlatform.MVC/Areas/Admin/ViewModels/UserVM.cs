using BloggingApp.Domain.Repositories;

namespace BloggingAppPlatform.MVC.Areas.Admin.ViewModels;

public class UserVM
{
    public List<UserDetail> Users { get; set; } = [];
}
