using Contracts.IRepositories;
using CustomizableForms.Application.Queries.UsersQueries;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Handlers.UsersHandlers;

public class GetUserByEmailHandler(
    IRepositoryManager repository)
    : IRequestHandler<GetUserByEmailQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var existingUser = await repository.User.GetUserByEmailAsync(request.Email);

        if (existingUser == null)
        {
            return new InvalidEmailBadRequestResponse();
        }
        
        return new ApiOkResponse<User>(existingUser);
    }
}