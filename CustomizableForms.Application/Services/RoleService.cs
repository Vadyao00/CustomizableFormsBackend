using Contracts.IRepositories;
using Contracts.IServices;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;

namespace CustomizableForms.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;

    public RoleService(IRepositoryManager repository, ILoggerManager logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ApiBaseResponse> GetAllRolesAsync()
    {
        try
        {
            var roles = await _repository.Role.GetAllRolesAsync(trackChanges: false);
            return new ApiOkResponse<IEnumerable<Role>>(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetAllRolesAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving roles: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetUserRolesAsync(Guid userId)
    {
        try
        {
            var user = await _repository.User.GetUserByIdAsync(userId, trackChanges: false);
            if (user == null)
            {
                return new ApiBadRequestResponse("User not found");
            }

            var roles = await _repository.Role.GetUserRolesAsync(userId, trackChanges: false);
            return new ApiOkResponse<IEnumerable<Role>>(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetUserRolesAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving user roles: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> AssignRoleToUserAsync(Guid userId, string roleName, User currentUser)
    {
        try
        {
            var currentUserRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            if (!currentUserRoles.Any(r => r.Name == "Admin"))
            {
                return new ApiBadRequestResponse("You do not have permission to assign roles");
            }

            var targetUser = await _repository.User.GetUserByIdAsync(userId, trackChanges: false);
            if (targetUser == null)
            {
                return new ApiBadRequestResponse("Target user not found");
            }

            var role = await _repository.Role.GetRoleByNameAsync(roleName, trackChanges: false);
            if (role == null)
            {
                return new ApiBadRequestResponse($"Role '{roleName}' not found");
            }

            var userRoles = await _repository.Role.GetUserRolesAsync(userId, trackChanges: false);
            if (userRoles.Any(r => r.Id == role.Id))
            {
                return new ApiBadRequestResponse($"User already has the '{roleName}' role");
            }

            _repository.Role.AssignRoleToUser(userId, role.Id);
            await _repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(AssignRoleToUserAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error assigning role to user: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> RemoveRoleFromUserAsync(Guid userId, string roleName, User currentUser)
    {
        try
        {
            var currentUserRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            if (!currentUserRoles.Any(r => r.Name == "Admin"))
            {
                return new ApiBadRequestResponse("You do not have permission to remove roles");
            }

            var targetUser = await _repository.User.GetUserByIdAsync(userId, trackChanges: false);
            if (targetUser == null)
            {
                return new ApiBadRequestResponse("Target user not found");
            }

            var role = await _repository.Role.GetRoleByNameAsync(roleName, trackChanges: false);
            if (role == null)
            {
                return new ApiBadRequestResponse($"Role '{roleName}' not found");
            }

            var userRoles = await _repository.Role.GetUserRolesAsync(userId, trackChanges: false);
            if (!userRoles.Any(r => r.Id == role.Id))
            {
                //return new ApiBadRequestResponse($"User does not have the '{roleName}' role");
                return new ApiOkResponse<bool>(true);
            }

            _repository.Role.RemoveRoleFromUser(userId, role.Id);
            await _repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(RemoveRoleFromUserAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error removing role from user: {ex.Message}");
        }
    }
}