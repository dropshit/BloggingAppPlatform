using BloggingAppPlatform.MVC.Models;

namespace BloggingAppPlatform.MVC.ViewModels;

public class UpdateCommentVM
{
    public UpdateCommentForm CommentDto { get; set; } = new();
}
