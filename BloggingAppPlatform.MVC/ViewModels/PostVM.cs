using BloggingApp.Domain.Entities;
using BloggingApp.Domain.Repositories;

namespace BloggingAppPlatform.MVC.ViewModels;

public class PostVM
{
    public List<PostDetail> Posts { get; set; } = [];
    public List<PostDetail> PostsByUser { get; set; } = [];
    public List<CommentDetail> Comments { get; set; } = [];
    public User? user { get; set; }
    public int CommentCount { get; set; }
    public int Count { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool IsFollow { get; set; }
    public int FollowerCount { get; set; }
}
