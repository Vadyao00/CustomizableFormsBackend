using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.AuthCommands;
using CustomizableForms.Domain.ConfigurationModels;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace CustomizableForms.Application.Handlers.AuthHandlers;

public class RefreshTokenHandler(
    IRepositoryManager repository,
    JwtConfiguration jwtConfiguration,
    ISender sender)
    : IRequestHandler<RefreshTokenCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = GetPrincipalFromExpiredToken(request.TokenDto.AccessToken);

        var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;
        var user = await repository.User.GetUserByEmailAsync(userEmail);

        if (user == null || user.RefreshToken != request.TokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow || !user.IsActive)
        {
            return new RefreshTokenBadRequestResponse();
        }

        user = user;

        var token = await sender.Send(new CreateTokenCommand(false), cancellationToken);

        return new ApiOkResponse<TokenDto>(token);
    }
    
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret)),
            ValidateLifetime = false,
            ValidIssuer = jwtConfiguration.ValidIssuer,
            ValidAudience = jwtConfiguration.ValidAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}