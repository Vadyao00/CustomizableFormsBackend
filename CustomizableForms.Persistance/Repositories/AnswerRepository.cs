using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class AnswerRepository(CustomizableFormsContext context) : RepositoryBase<Answer>(context), IAnswerRepository
{
    public async Task<IEnumerable<Answer>> GetFormAnswersAsync(Guid formId, bool trackChanges)
    {
        return await FindByCondition(a => a.FormId == formId, trackChanges)
            .Include(a => a.Question)
            .ToListAsync();
    }

    public async Task<Answer> GetAnswerByIdAsync(Guid answerId, bool trackChanges)
    {
        return await FindByCondition(a => a.Id == answerId, trackChanges)
            .Include(a => a.Question)
            .FirstOrDefaultAsync();
    }

    public void CreateAnswer(Answer answer) => Create(answer);

    public void UpdateAnswer(Answer answer) => Update(answer);

    public void DeleteAnswer(Answer answer) => Delete(answer);
}