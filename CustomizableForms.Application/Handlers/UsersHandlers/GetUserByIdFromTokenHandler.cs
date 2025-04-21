using Contracts.IRepositories;
using CustomizableForms.Application.Queries.UsersQueries;
using CustomizableForms.Domain.Entities;
using MediatR;

namespace CustomizableForms.Application.Handlers.UsersHandlers;

public class GetUserByIdFromTokenHandler(
    IRepositoryManager repository)
    : IRequestHandler<GetUserByIdFromTokenQuery, User?>
{
    public async Task<User?> Handle(GetUserByIdFromTokenQuery request, CancellationToken cancellationToken)
    {
        return await repository.User.GetUserByIdAsync(request.UserId, trackChanges: false);
    }
}