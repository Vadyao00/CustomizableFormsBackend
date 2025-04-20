using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;

namespace Contracts.IRepositories;

public interface IFormRepository
{
    Task<PagedList<Form>> GetUserFormsAsync(FormParameters formParameters, Guid userId, bool trackChanges);
    Task<PagedList<Form>> GetTemplateFormsAsync(FormParameters formParameters, Guid templateId, bool trackChanges);
    Task<IEnumerable<Form>> GetAllTemplateFormsAsync(Guid templateId, bool trackChanges);
    Task<Form> GetFormByIdAsync(Guid formId, bool trackChanges);
    void CreateForm(Form form);
    void UpdateForm(Form form);
    void DeleteForm(Form form);
}