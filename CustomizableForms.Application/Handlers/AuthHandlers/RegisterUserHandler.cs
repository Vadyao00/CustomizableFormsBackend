using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.AuthCommands;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.AuthHandlers;

public class RegisterUserHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<RegisterUserCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.UserForRegistration.Password);
        
        var user = mapper.Map<User>(request.UserForRegistration);
        user.PasswordHash = passwordHash;
        try
        {
            repository.User.CreateUser(user);
            await repository.SaveAsync();
            
            var userRole = await repository.Role.GetRoleByNameAsync("User", trackChanges: false);
            if (userRole != null)
            {
                repository.Role.AssignRoleToUser(user.Id, userRole.Id);
                await repository.SaveAsync();
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Error registering user: {e.Message}");
            return new InvalidEmailBadRequestResponse();
        }

        return new ApiOkResponse<User>(user);
    }
}