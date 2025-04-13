using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class LikeRepository : RepositoryBase<TemplateLike>, ILikeRepository
{
    public LikeRepository(CustomizableFormsContext context) : base(context)
    {
    }

    public async Task<bool> HasUserLikedTemplateAsync(Guid userId, Guid templateId, bool trackChanges)
    {
        return await FindByCondition(l => l.UserId == userId && l.TemplateId == templateId, trackChanges)
            .AnyAsync();
    }

    public async Task<int> GetTemplateLikesCountAsync(Guid templateId)
    {
        return await FindByCondition(l => l.TemplateId == templateId, false)
            .CountAsync();
    }

    public void CreateLike(TemplateLike like) => Create(like);

    public void DeleteLike(TemplateLike like) => Delete(like);
}