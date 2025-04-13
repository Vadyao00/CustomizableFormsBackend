using CustomizableForms.Domain.Entities;

namespace Contracts.IRepositories;

public interface IFormRepository
{
    Task<IEnumerable<Form>> GetUserFormsAsync(Guid userId, bool trackChanges);
    Task<IEnumerable<Form>> GetTemplateFormsAsync(Guid templateId, bool trackChanges);
    Task<Form> GetFormByIdAsync(Guid formId, bool trackChanges);
    void CreateForm(Form form);
    void UpdateForm(Form form);
    void DeleteForm(Form form);
}