using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class CommentRepository : RepositoryBase<TemplateComment>, ICommentRepository
{
    public CommentRepository(CustomizableFormsContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TemplateComment>> GetTemplateCommentsAsync(Guid templateId, bool trackChanges)
    {
        return await FindByCondition(c => c.TemplateId == templateId, trackChanges)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<TemplateComment> GetCommentByIdAsync(Guid commentId, bool trackChanges)
    {
        return await FindByCondition(c => c.Id == commentId, trackChanges)
            .Include(c => c.User)
            .Include(c => c.Template)
            .FirstOrDefaultAsync();
    }

    public void CreateComment(TemplateComment comment) => Create(comment);

    public void UpdateComment(TemplateComment comment) => Update(comment);

    public void DeleteComment(TemplateComment comment) => Delete(comment);
}