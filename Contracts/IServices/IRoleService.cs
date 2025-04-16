using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface IRoleService
{
    Task<ApiBaseResponse> GetUserRolesAsync(Guid userId);
    Task<ApiBaseResponse> AssignRoleToUserAsync(Guid userId, string roleName);
    Task<ApiBaseResponse> RemoveRoleFromUserAsync(Guid userId, string roleName);
}