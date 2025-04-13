using CustomizableForms.Domain.Entities;

namespace Contracts.IRepositories;

public interface ITemplateRepository
{
    Task<IEnumerable<Template>> GetAllTemplatesAsync(bool trackChanges);
    Task<IEnumerable<Template>> GetPublicTemplatesAsync(bool trackChanges);
    Task<IEnumerable<Template>> GetUserTemplatesAsync(Guid userId, bool trackChanges);
    Task<IEnumerable<Template>> GetAccessibleTemplatesAsync(Guid userId, bool trackChanges);
    Task<IEnumerable<Template>> GetPopularTemplatesAsync(int count, bool trackChanges);
    Task<IEnumerable<Template>> GetRecentTemplatesAsync(int count, bool trackChanges);
    Task<IEnumerable<Template>> SearchTemplatesAsync(string searchTerm, bool trackChanges);
    Task<Template> GetTemplateByIdAsync(Guid templateId, bool trackChanges);
    void CreateTemplate(Template template);
    void UpdateTemplate(Template template);
    void DeleteTemplate(Template template);
}