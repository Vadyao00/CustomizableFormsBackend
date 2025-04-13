using CustomizableForms.Domain.Entities;

namespace Contracts.IRepositories;

public interface ILikeRepository
{
    Task<bool> HasUserLikedTemplateAsync(Guid userId, Guid templateId, bool trackChanges);
    Task<int> GetTemplateLikesCountAsync(Guid templateId);
    void CreateLike(TemplateLike like);
    void DeleteLike(TemplateLike like);
}