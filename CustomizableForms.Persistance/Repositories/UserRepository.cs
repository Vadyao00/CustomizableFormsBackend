using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(CustomizableFormsContext dbContext) : base(dbContext)
    {
    }

    public async Task<PagedList<User>> GetAllUsersAsync(UserParameters userParameters, bool trackChanges)
    {
        var users = await FindAll(trackChanges)
            .OrderBy(u => u.Name)
            .Skip((userParameters.PageNumber - 1) * userParameters.PageSize)
            .Take(userParameters.PageSize)
            .ToListAsync();

        var count = await FindAll(trackChanges).CountAsync();

        return new PagedList<User>(users, count, userParameters.PageNumber, userParameters.PageSize);
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