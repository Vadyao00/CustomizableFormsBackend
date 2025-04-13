using CustomizableForms.Domain.Entities;

namespace Contracts.IRepositories;

public interface IAnswerRepository
{
    Task<IEnumerable<Answer>> GetFormAnswersAsync(Guid formId, bool trackChanges);
    Task<Answer> GetAnswerByIdAsync(Guid answerId, bool trackChanges);
    void CreateAnswer(Answer answer);
    void UpdateAnswer(Answer answer);
    void DeleteAnswer(Answer answer);
}