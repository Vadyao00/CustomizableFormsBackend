using Contracts.IRepositories;
using CustomizableForms.Application.Commands.RolesCommands;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.RolesHandlers;

public class AssignRoleToUserHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<AssignRoleToUserCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var targetUser = await repository.User.GetUserByIdAsync(request.UserId, trackChanges: false);
            if (targetUser == null)
            {
                return new ApiBadRequestResponse("Target user not found");
            }

            var role = await repository.Role.GetRoleByNameAsync(request.RoleName, trackChanges: false);
            if (role == null)
            {
                return new ApiBadRequestResponse($"Role '{request.RoleName}' not found");
            }

            var userRoles = await repository.Role.GetUserRolesAsync(request.UserId, trackChanges: false);
            if (userRoles.Any(r => r.Id == role.Id))
            {
                return new ApiBadRequestResponse($"User already has the '{request.RoleName}' role");
            }

            repository.Role.AssignRoleToUser(request.UserId, role.Id);
            await repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(AssignRoleToUserHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error assigning role to user: {ex.Message}");
        }
    }
}