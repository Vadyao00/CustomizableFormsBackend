using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class UserSalesforceProfileRepository : RepositoryBase<UserSalesforceProfile>, IUserSalesforceProfileRepository
{
    public UserSalesforceProfileRepository(CustomizableFormsContext context) : base(context)
    {
    }

    public async Task<UserSalesforceProfile> GetByUserIdAsync(Guid userId, bool trackChanges)
    {
        return await FindByCondition(p => p.UserId == userId, trackChanges)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsForUserAsync(Guid userId)
    {
        return await FindByCondition(p => p.UserId == userId, false)
            .AnyAsync();
    }

    public void CreateProfile(UserSalesforceProfile profile) => Create(profile);

    public void UpdateProfile(UserSalesforceProfile profile) => Update(profile);

    public void DeleteProfile(UserSalesforceProfile profile) => Delete(profile);
}