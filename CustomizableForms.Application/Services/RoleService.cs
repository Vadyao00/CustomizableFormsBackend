using Contracts.IRepositories;
using Contracts.IServices;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;

namespace CustomizableForms.Application.Services;

public class RoleService(IRepositoryManager repository, ILoggerManager logger) : IRoleService
{
    public async Task<ApiBaseResponse> GetUserRolesAsync(Guid userId)
    {
        try
        {
            var user = await repository.User.GetUserByIdAsync(userId, trackChanges: false);
            if (user == null)
            {
                return new ApiBadRequestResponse("User not found");
            }

            var roles = await repository.Role.GetUserRolesAsync(userId, trackChanges: false);
            return new ApiOkResponse<IEnumerable<Role>>(roles);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetUserRolesAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving user roles: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> AssignRoleToUserAsync(Guid userId, string roleName)
    {
        try
        {
            var targetUser = await repository.User.GetUserByIdAsync(userId, trackChanges: false);
            if (targetUser == null)
            {
                return new ApiBadRequestResponse("Target user not found");
            }

            var role = await repository.Role.GetRoleByNameAsync(roleName, trackChanges: false);
            if (role == null)
            {
                return new ApiBadRequestResponse($"Role '{roleName}' not found");
            }

            var userRoles = await repository.Role.GetUserRolesAsync(userId, trackChanges: false);
            if (userRoles.Any(r => r.Id == role.Id))
            {
                return new ApiBadRequestResponse($"User already has the '{roleName}' role");
            }

            repository.Role.AssignRoleToUser(userId, role.Id);
            await repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(AssignRoleToUserAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error assigning role to user: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> RemoveRoleFromUserAsync(Guid userId, string roleName)
    {
        try
        {
            var targetUser = await repository.User.GetUserByIdAsync(userId, trackChanges: false);
            if (targetUser == null)
            {
                return new ApiBadRequestResponse("Target user not found");
            }

            var role = await repository.Role.GetRoleByNameAsync(roleName, trackChanges: false);
            if (role == null)
            {
                return new ApiBadRequestResponse($"Role '{roleName}' not found");
            }

            var userRoles = await repository.Role.GetUserRolesAsync(userId, trackChanges: false);
            if (!userRoles.Any(r => r.Id == role.Id))
            {
                //return new ApiBadRequestResponse($"User does not have the '{roleName}' role");
                return new ApiOkResponse<bool>(true);
            }

            repository.Role.RemoveRoleFromUser(userId, role.Id);
            await repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(RemoveRoleFromUserAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error removing role from user: {ex.Message}");
        }
    }
}