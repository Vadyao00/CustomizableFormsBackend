using AutoMapper;
using Contracts.IRepositories;
using Contracts.IServices;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;

namespace CustomizableForms.Application.Services;

public class UserService : IUserService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IMapper _mapper;

    public UserService(IRepositoryManager repositoryManager, IMapper mapper)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
    }

    public async Task<ApiBaseResponse> GetUserByEmailAsync(string email, User currentUser)
    {
        if (!CheckUser(currentUser))
            return new BadUserBadRequestResponse();
        
        var existingUser = await _repositoryManager.User.GetUserByEmailAsync(email);

        if (existingUser == null)
        {
            return new InvalidEmailBadRequestResponse();
        }
        
        return new ApiOkResponse<User>(existingUser);
    }
    
    public async Task<ApiBaseResponse> GetUserByEmailWithoutCurrentUserAsync(string email)
    {
        var existingUser = await _repositoryManager.User.GetUserByEmailAsync(email);

        if (existingUser == null)
        {
            return new InvalidEmailBadRequestResponse();
        }
        
        return new ApiOkResponse<User>(existingUser);
    }
    
    public async Task<ApiBaseResponse> GetUserByIdAsync(Guid userId, User currentUser)
    {
        if (!CheckUser(currentUser))
            return new BadUserBadRequestResponse();
        
        var existingUser = await _repositoryManager.User.GetUserByIdAsync(userId, trackChanges: false);

        if (existingUser == null)
        {
            return new ApiBadRequestResponse("User not found");
        }
        
        return new ApiOkResponse<User>(existingUser);
    }

    public async Task<User?> GetUserByIdFromTokenAsync(Guid userId)
    {
        return await _repositoryManager.User.GetUserByIdAsync(userId, trackChanges: false);
    }
    
    public async Task<ApiBaseResponse> DeleteUserAsync(Guid id, User currentUser)
    {
        if (!CheckUser(currentUser))
            return new BadUserBadRequestResponse();
        
        var user = await _repositoryManager.User.GetUserByIdAsync(id, trackChanges: false);
        if (user != null)
        {
            _repositoryManager.User.DeleteUser(user);
            await _repositoryManager.SaveAsync();
        }
        else
        {
            return new InvalidEmailBadRequestResponse();
        }

        return new ApiOkResponse<User>(user);
    }
    
    public async Task<ApiBaseResponse> GetAllUsersAsync(User currentUser)
    {
        if (!CheckUser(currentUser))
            return new BadUserBadRequestResponse();
        
        var currentUserRoles = await _repositoryManager.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
        if (!currentUserRoles.Any(r => r.Name == "Admin"))
        {
            return new ApiBadRequestResponse("You do not have permission to view all users");
        }
        
        var users = await _repositoryManager.User.GetAllUsersAsync(trackChanges: false);
        var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
        
        foreach (var userDto in usersDto)
        {
            var user = users.FirstOrDefault(u => u.Id.ToString() == userDto.Id);
            if (user != null)
            {
                var roles = await _repositoryManager.Role.GetUserRolesAsync(user.Id, trackChanges: false);
                userDto.Roles = roles.Select(r => r.Name).ToList();
            }
        }
        
        return new ApiOkResponse<IEnumerable<UserDto>>(usersDto);
    }
    
    public async Task<ApiBaseResponse> BlockUserAsync(Guid userId, User currentUser)
    {
        if (!CheckUser(currentUser))
            return new BadUserBadRequestResponse();
        
        var currentUserRoles = await _repositoryManager.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
        if (!currentUserRoles.Any(r => r.Name == "Admin"))
        {
            return new ApiBadRequestResponse("You do not have permission to block users");
        }
        
        var user = await _repositoryManager.User.GetUserByIdAsync(userId, trackChanges: true);
        if (user == null)
        {
            return new ApiBadRequestResponse("User not found");
        }
        
        if (user.Id == currentUser.Id)
        {
            return new ApiBadRequestResponse("You cannot block yourself");
        }
        
        user.IsActive = false;
        _repositoryManager.User.UpdateUser(user);
        await _repositoryManager.SaveAsync();
        
        return new ApiOkResponse<bool>(true);
    }
    
    public async Task<ApiBaseResponse> UnblockUserAsync(Guid userId, User currentUser)
    {
        if (!CheckUser(currentUser))
            return new BadUserBadRequestResponse();
        
        var currentUserRoles = await _repositoryManager.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
        if (!currentUserRoles.Any(r => r.Name == "Admin"))
        {
            return new ApiBadRequestResponse("You do not have permission to unblock users");
        }
        
        var user = await _repositoryManager.User.GetUserByIdAsync(userId, trackChanges: true);
        if (user == null)
        {
            return new ApiBadRequestResponse("User not found");
        }
        
        user.IsActive = true;
        _repositoryManager.User.UpdateUser(user);
        await _repositoryManager.SaveAsync();
        
        return new ApiOkResponse<bool>(true);
    }
    
    private bool CheckUser(User? user)
    {
        if (user == null || !user.IsActive)
        {
            return false;
        }

        return true;
    }
}