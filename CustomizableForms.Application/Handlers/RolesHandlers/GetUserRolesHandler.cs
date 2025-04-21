using Contracts.IRepositories;
using CustomizableForms.Application.Queries.RolesQueries;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.RolesHandlers;

public class GetUserRolesHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<GetUserRolesQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await repository.User.GetUserByIdAsync(request.UserId, trackChanges: false);
            if (user == null)
            {
                return new ApiBadRequestResponse("User not found");
            }

            var roles = await repository.Role.GetUserRolesAsync(request.UserId, trackChanges: false);
            return new ApiOkResponse<IEnumerable<Role>>(roles);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetUserRolesHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving user roles: {ex.Message}");
        }
    }
}