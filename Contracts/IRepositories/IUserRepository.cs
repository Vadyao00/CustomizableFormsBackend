using CustomizableForms.Domain.Entities;

namespace Contracts.IRepositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync(bool trackChanges);
    Task<User?> GetUserByIdAsync(Guid userId, bool trackChanges);
    Task<User?> GetUserByEmailAsync(string email);
    void CreateUser(User user);
    void DeleteUser(User user);
    void UpdateUser(User user);
}