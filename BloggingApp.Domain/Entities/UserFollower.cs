namespace BloggingApp.Domain.Entities;

public class UserFollower : BaseEntity
{
    public int FollowerId { get; set; }
    public int FollowedUserId { get; set; }
}
