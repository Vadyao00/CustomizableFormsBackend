using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface IUserService
{
    Task<ApiBaseResponse> GetUserByEmailAsync(string email, User currentUser);
    Task<ApiBaseResponse> GetUserByIdAsync(Guid userId, User currentUser);
    Task<ApiBaseResponse> DeleteUserAsync(Guid id, User currentUser);
    Task<User?> GetUserByIdFromTokenAsync(Guid userId);
    Task<ApiBaseResponse> GetAllUsersAsync(User currentUser);
    Task<ApiBaseResponse> BlockUserAsync(Guid userId, User currentUser);
    Task<ApiBaseResponse> UnblockUserAsync(Guid userId, User currentUser);
}