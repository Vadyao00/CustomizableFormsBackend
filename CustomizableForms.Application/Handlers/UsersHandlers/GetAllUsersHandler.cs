using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.UsersQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Handlers.UsersHandlers;

public class GetAllUsersHandler(
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetAllUsersQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await repository.User.GetAllUsersAsync(request.UserParameters, trackChanges: false);
        var usersDto = mapper.Map<IEnumerable<UserDto>>(users);
        
        foreach (var userDto in usersDto)
        {
            var user = users.FirstOrDefault(u => u.Id.ToString() == userDto.Id);
            if (user != null)
            {
                var roles = await repository.Role.GetUserRolesAsync(user.Id, trackChanges: false);
                userDto.Roles = roles.Select(r => r.Name).ToList();
            }
        }
        
        return new ApiOkResponse<(IEnumerable<UserDto>, MetaData)>((usersDto, users.MetaData));
    }
}