using BloggingApp.Application.Common.Interfaces;
using BloggingApp.Application.Common.Responses;
using BloggingApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BloggingApp.Infrastructure.Auth;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly TokenOptions _options =
        configuration.GetSection("TokenOptions").Get<TokenOptions>()
        ?? throw new InvalidOperationException("TokenOptions section is missing from configuration.");

    public AccessTokenResponse CreateToken(User user, List<OperationClaim> claims)
    {
        var expiration = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpiration);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var jwtClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Username)
        };
        jwtClaims.AddRange(claims.Select(c => new Claim(ClaimTypes.Role, c.Name)));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: jwtClaims,
            notBefore: DateTime.UtcNow,
            expires: expiration,
            signingCredentials: credentials);

        return new AccessTokenResponse(new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }
}
