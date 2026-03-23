using BloggingApp.Domain.Entities;
using BloggingApp.Application.Common.Responses;

namespace BloggingApp.Application.Common.Interfaces;

public interface IJwtService
{
    AccessTokenResponse CreateToken(User user, List<OperationClaim> claims);
}
