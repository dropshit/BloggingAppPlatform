namespace BloggingAppPlatform.MVC.Models;

public class RegisterForm
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RePassword { get; set; } = string.Empty;
}

public class LoginForm
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AddPostForm
{
    public string Title { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
}

public class UpdatePostForm
{
    public int PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
}

public class AddCommentForm
{
    public int PostId { get; set; }
    public string CommentText { get; set; } = string.Empty;
}

public class UpdateCommentForm
{
    public int CommentId { get; set; }
    public string CommentText { get; set; } = string.Empty;
}

public class UpdateUserForm
{
    public string Username { get; set; } = string.Empty;
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class OpClaimForm
{
    public string Username { get; set; } = string.Empty;
    public string ClaimName { get; set; } = string.Empty;
}
