using CustomizableForms.Domain.Entities;

namespace Contracts.IRepositories;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllRolesAsync(bool trackChanges);
    Task<Role> GetRoleByIdAsync(Guid roleId, bool trackChanges);
    Task<Role> GetRoleByNameAsync(string name, bool trackChanges);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId, bool trackChanges);
    void CreateRole(Role role);
    void UpdateRole(Role role);
    void DeleteRole(Role role);
    void AssignRoleToUser(Guid userId, Guid roleId);
    void RemoveRoleFromUser(Guid userId, Guid roleId);
}