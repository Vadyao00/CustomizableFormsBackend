using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface IAuthenticationService
{
    Task<ApiBaseResponse> RegisterUser(UserForRegistrationDto userForRegistrationDto);
    Task<ApiBaseResponse> ValidateUser(UserForAuthenticationDto userForAuth);
    Task<TokenDto> CreateToken(bool populateExp);
    Task<ApiBaseResponse> RefreshToken(TokenDto tokenDto);
    Task<User> GetCurrentUserFromTokenAsync(string token);
}