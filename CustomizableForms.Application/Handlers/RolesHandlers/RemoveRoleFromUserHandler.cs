using Contracts.IRepositories;
using CustomizableForms.Application.Commands.RolesCommands;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.RolesHandlers;

public class RemoveRoleFromUserHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<RemoveRoleFromUserCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
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
            if (!userRoles.Any(r => r.Id == role.Id))
            {
                //return new ApiBadRequestResponse($"User does not have the '{roleName}' role");
                return new ApiOkResponse<bool>(true);
            }

            repository.Role.RemoveRoleFromUser(request.UserId, role.Id);
            await repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(RemoveRoleFromUserHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error removing role from user: {ex.Message}");
        }
    }
}