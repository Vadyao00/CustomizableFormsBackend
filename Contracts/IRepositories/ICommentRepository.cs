using CustomizableForms.Domain.Entities;

namespace Contracts.IRepositories;

public interface ICommentRepository
{
    Task<IEnumerable<TemplateComment>> GetTemplateCommentsAsync(Guid templateId, bool trackChanges);
    Task<TemplateComment> GetCommentByIdAsync(Guid commentId, bool trackChanges);
    void CreateComment(TemplateComment comment);
    void UpdateComment(TemplateComment comment);
    void DeleteComment(TemplateComment comment);
}