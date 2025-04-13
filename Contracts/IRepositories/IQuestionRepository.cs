using CustomizableForms.Domain.Entities;

namespace Contracts.IRepositories;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetTemplateQuestionsAsync(Guid templateId, bool trackChanges);
    Task<Question> GetQuestionByIdAsync(Guid questionId, bool trackChanges);
    void CreateQuestion(Question question);
    void UpdateQuestion(Question question);
    void DeleteQuestion(Question question);
}