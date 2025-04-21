using Contracts.IRepositories;
using CustomizableForms.Application.Commands.UsersCommands;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Handlers.UsersHandlers;

public class BlockUserHandler(
    IRepositoryManager repository)
    : IRequestHandler<BlockUserCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.User.GetUserByIdAsync(request.UserId, trackChanges: true);
        if (user == null)
        {
            return new ApiBadRequestResponse("User not found");
        }
        
        if (user.Id == request.CurrentUser.Id)
        {
            return new ApiBadRequestResponse("You cannot block yourself");
        }
        
        user.IsActive = false;
        repository.User.UpdateUser(user);
        await repository.SaveAsync();
        
        return new ApiOkResponse<bool>(true);
    }
}