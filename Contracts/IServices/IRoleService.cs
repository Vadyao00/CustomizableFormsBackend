using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface IRoleService
{
    Task<ApiBaseResponse> GetAllRolesAsync();
    Task<ApiBaseResponse> GetUserRolesAsync(Guid userId);
    Task<ApiBaseResponse> AssignRoleToUserAsync(Guid userId, string roleName, User currentUser);
    Task<ApiBaseResponse> RemoveRoleFromUserAsync(Guid userId, string roleName, User currentUser);
}