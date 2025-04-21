using Contracts.IRepositories;
using CustomizableForms.Application.Commands.UsersCommands;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Handlers.UsersHandlers;

public class DeleteUserHandler(
    IRepositoryManager repository)
    : IRequestHandler<DeleteUserCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.User.GetUserByIdAsync(request.Id, trackChanges: false);
        if (user != null)
        {
            repository.User.DeleteUser(user);
            await repository.SaveAsync();
        }
        else
        {
            return new InvalidEmailBadRequestResponse();
        }

        return new ApiOkResponse<User>(user);
    }
}