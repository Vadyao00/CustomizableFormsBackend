using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class FormRepository : RepositoryBase<Form>, IFormRepository
{
    public FormRepository(CustomizableFormsContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Form>> GetUserFormsAsync(Guid userId, bool trackChanges)
    {
        return await FindByCondition(f => f.UserId == userId, trackChanges)
            .Include(f => f.Template)
            .Include(f => f.User)
            .OrderByDescending(f => f.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Form>> GetTemplateFormsAsync(Guid templateId, bool trackChanges)
    {
        return await FindByCondition(f => f.TemplateId == templateId, trackChanges)
            .Include(f => f.User)
            .Include(f => f.Answers)
            .ThenInclude(a => a.Question)
            .OrderByDescending(f => f.SubmittedAt)
            .ToListAsync();
    }

    public async Task<Form> GetFormByIdAsync(Guid formId, bool trackChanges)
    {
        return await FindByCondition(f => f.Id == formId, trackChanges)
            .Include(f => f.Template)
            .ThenInclude(t => t.Creator)
            .Include(f => f.User)
            .Include(f => f.Answers)
            .ThenInclude(a => a.Question)
            .FirstOrDefaultAsync();
    }

    public void CreateForm(Form form) => Create(form);

    public void UpdateForm(Form form) => Update(form);

    public void DeleteForm(Form form) => Delete(form);
}