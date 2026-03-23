using BloggingAppPlatform.MVC.Models;

namespace BloggingAppPlatform.MVC.ViewModels;

public class UpdatePostVm
{
    public UpdatePostForm postDto { get; set; } = new();
}
