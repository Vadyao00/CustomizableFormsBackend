using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;

namespace Contracts.IRepositories;

public interface IUserRepository
{
    Task<PagedList<User>> GetAllUsersAsync(UserParameters userParameters, bool trackChanges);
    Task<User?> GetUserByIdAsync(Guid userId, bool trackChanges);
    Task<User?> GetUserByEmailAsync(string email);
    void CreateUser(User user);
    void DeleteUser(User user);
    void UpdateUser(User user);
}