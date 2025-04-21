using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.AuthCommands;
using CustomizableForms.Domain.ConfigurationModels;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace CustomizableForms.Application.Handlers.AuthHandlers;

public class CreateTokenHandler(
    IRepositoryManager repository,
    JwtConfiguration jwtConfiguration,
    User? user)
    : IRequestHandler<CreateTokenCommand, TokenDto>
{
    public async Task<TokenDto> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        repository.User.UpdateUser(user);
        await repository.SaveAsync();

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, refreshToken);
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(jwtConfiguration.Secret);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
    
    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("PreferredLanguage", user.PreferredLanguage),
            new Claim("PreferredTheme", user.PreferredTheme) 
        };
        
        var userRoles = await repository.Role.GetUserRolesAsync(user.Id, trackChanges: false);
        
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        return claims;
    }
    
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken
        (
            issuer: jwtConfiguration.ValidIssuer,
            audience: jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtConfiguration.Expires)),
            signingCredentials: signingCredentials
        );

        return tokenOptions;
    }
    
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}