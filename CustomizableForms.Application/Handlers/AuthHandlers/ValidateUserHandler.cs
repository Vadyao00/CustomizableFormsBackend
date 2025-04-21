using Contracts.IRepositories;
using CustomizableForms.Application.Commands.AuthCommands;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Handlers.AuthHandlers;

public class ValidateUserHandler(
    IRepositoryManager repository,
    User? user)
    : IRequestHandler<ValidateUserCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(ValidateUserCommand request, CancellationToken cancellationToken)
    {
        user = await repository.User.GetUserByEmailAsync(request.UserForAuth.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.UserForAuth.Password, user.PasswordHash))
        {
            return new BadUserBadRequestResponse();
        }

        if(!user.IsActive)
            return new BlockedUserBadRequestResponse();
        
        return new ApiOkResponse<User>(user);
    }
}