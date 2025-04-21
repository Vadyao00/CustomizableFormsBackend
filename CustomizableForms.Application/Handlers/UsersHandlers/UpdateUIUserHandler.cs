using Contracts.IRepositories;
using CustomizableForms.Application.Commands.UsersCommands;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Handlers.UsersHandlers;

public class UpdateUIUserHandler(
    IRepositoryManager repository)
    : IRequestHandler<UpdateUIUserCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(UpdateUIUserCommand request, CancellationToken cancellationToken)
    {
        if(request.PrefLang is not null)
            request.CurrentUser.PreferredLanguage = request.PrefLang;
        if(request.PrefTheme is not null)
            request.CurrentUser.PreferredTheme = request.PrefTheme;
        repository.User.UpdateUser(request.CurrentUser);
        await repository.SaveAsync();
        
        return new ApiOkResponse<bool>(true);
    }
}