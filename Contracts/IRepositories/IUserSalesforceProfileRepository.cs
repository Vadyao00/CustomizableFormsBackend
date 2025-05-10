using CustomizableForms.Domain.Entities;

namespace Contracts.IRepositories;

public interface IUserSalesforceProfileRepository
{
    Task<UserSalesforceProfile> GetByUserIdAsync(Guid userId, bool trackChanges);
    Task<bool> ExistsForUserAsync(Guid userId);
    void CreateProfile(UserSalesforceProfile profile);
    void UpdateProfile(UserSalesforceProfile profile);
    void DeleteProfile(UserSalesforceProfile profile);
}