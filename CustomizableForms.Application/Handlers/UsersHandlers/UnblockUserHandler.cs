using Contracts.IRepositories;
using CustomizableForms.Application.Commands.UsersCommands;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Handlers.UsersHandlers;

public class UnblockUserHandler(
    IRepositoryManager repository)
    : IRequestHandler<UnblockUserCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.User.GetUserByIdAsync(request.UserId, trackChanges: true);
        if (user == null)
        {
            return new ApiBadRequestResponse("User not found");
        }
        
        user.IsActive = true;
        repository.User.UpdateUser(user);
        await repository.SaveAsync();
        
        return new ApiOkResponse<bool>(true);
    }
}