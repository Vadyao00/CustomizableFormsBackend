using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface IUserService
{
    Task<ApiBaseResponse> GetUserByEmailAsync(string email);
    Task<ApiBaseResponse> GetUserByIdAsync(Guid userId);
    Task<ApiBaseResponse> DeleteUserAsync(Guid id);
    Task<User?> GetUserByIdFromTokenAsync(Guid userId);
    Task<ApiBaseResponse> GetAllUsersAsync();
    Task<ApiBaseResponse> BlockUserAsync(Guid userId, User currentUser);
    Task<ApiBaseResponse> UnblockUserAsync(Guid userId);
    Task<ApiBaseResponse> UpdateUIUserAsync(string prefTheme, string prefLang, User currentUser);
}