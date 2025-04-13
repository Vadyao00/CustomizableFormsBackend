using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class RoleRepository : RepositoryBase<Role>, IRoleRepository
{
    public RoleRepository(CustomizableFormsContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync(bool trackChanges)
    {
        return await FindAll(trackChanges)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Role> GetRoleByIdAsync(Guid roleId, bool trackChanges)
    {
        return await FindByCondition(r => r.Id == roleId, trackChanges)
            .FirstOrDefaultAsync();
    }

    public async Task<Role> GetRoleByNameAsync(string name, bool trackChanges)
    {
        return await FindByCondition(r => r.Name == name, trackChanges)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId, bool trackChanges)
    {
        var userRoles = await DbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync();

        return userRoles;
    }

    public void CreateRole(Role role) => Create(role);

    public void UpdateRole(Role role) => Update(role);

    public void DeleteRole(Role role) => Delete(role);

    public void AssignRoleToUser(Guid userId, Guid roleId)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        DbContext.UserRoles.Add(userRole);
    }

    public void RemoveRoleFromUser(Guid userId, Guid roleId)
    {
        var userRole = DbContext.UserRoles
            .FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole != null)
        {
            DbContext.UserRoles.Remove(userRole);
        }
    }
}