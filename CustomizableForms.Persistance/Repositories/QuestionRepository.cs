using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class QuestionRepository : RepositoryBase<Question>, IQuestionRepository
{
    public QuestionRepository(CustomizableFormsContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Question>> GetTemplateQuestionsAsync(Guid templateId, bool trackChanges)
    {
        return await FindByCondition(q => q.TemplateId == templateId, trackChanges)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync();
    }

    public async Task<Question> GetQuestionByIdAsync(Guid questionId, bool trackChanges)
    {
        return await FindByCondition(q => q.Id == questionId, trackChanges)
            .FirstOrDefaultAsync();
    }

    public void CreateQuestion(Question question) => Create(question);

    public void UpdateQuestion(Question question) => Update(question);

    public void DeleteQuestion(Question question) => Delete(question);
}