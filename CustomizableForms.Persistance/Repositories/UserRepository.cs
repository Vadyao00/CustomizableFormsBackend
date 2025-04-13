using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(CustomizableFormsContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(bool trackChanges)
    {
        return await FindAll(trackChanges)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, bool trackChanges)
    {
        return await FindByCondition(u => u.Id == userId, trackChanges)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var existingUser = await FindByCondition(u => u.Email == email, false).FirstOrDefaultAsync();

        return existingUser;
    }

    public void CreateUser(User user) => Create(user);

    public void DeleteUser(User user) => Delete(user);

    public void UpdateUser(User user) => Update(user);
}