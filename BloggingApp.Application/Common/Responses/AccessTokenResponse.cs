namespace BloggingApp.Application.Common.Responses;

public record AccessTokenResponse(string Token, DateTime Expiration);
