using Contracts.IRepositories;
using CustomizableForms.Application.Queries.UsersQueries;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Handlers.UsersHandlers;

public class GetUserByIdHandler(
    IRepositoryManager repository)
    : IRequestHandler<GetUserByIdQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var existingUser = await repository.User.GetUserByIdAsync(request.UserId, trackChanges: false);

        if (existingUser == null)
        {
            return new ApiBadRequestResponse("User not found");
        }
        
        return new ApiOkResponse<User>(existingUser);
    }
}