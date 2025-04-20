using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;

namespace Contracts.IRepositories;

public interface ITemplateRepository
{
    Task<IEnumerable<Template>> GetPublicTemplatesAsync(bool trackChanges);
    Task<IEnumerable<Template>> GetAllowedTemplatesAsync(User currentUser, bool isAdmin, bool trackChanges);
    Task<PagedList<Template>> GetUserTemplatesAsync(TemplateParameters templateParameters, Guid userId, bool trackChanges);
    Task<IEnumerable<Template>> GetPopularTemplatesAsync(int count, bool trackChanges);
    Task<IEnumerable<Template>> GetRecentTemplatesAsync(int count, bool trackChanges);
    Task<IEnumerable<Template>> SearchTemplatesAsync(string searchTerm, bool trackChanges);
    Task<Template> GetTemplateByIdAsync(Guid templateId, bool trackChanges);
    void CreateTemplate(Template template);
    void UpdateTemplate(Template template);
    void DeleteTemplate(Template template);
}