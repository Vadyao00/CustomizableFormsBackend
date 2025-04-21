using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;

namespace Contracts.IRepositories;

public interface ITemplateRepository
{
    Task<PagedList<Template>> GetTemplatesByTagAsync(TemplateParameters templateParameters, string tagName, User currentUser, bool isAdmin, bool trackChanges);
    Task<PagedList<Template>> GetPublicTemplatesAsync(TemplateParameters templateParameters, bool trackChanges);
    Task<PagedList<Template>> GetAllowedTemplatesAsync(TemplateParameters templateParameters, User currentUser, bool isAdmin, bool trackChanges);
    Task<PagedList<Template>> GetUserTemplatesAsync(TemplateParameters templateParameters, Guid userId, bool trackChanges);
    Task<IEnumerable<Template>> GetPopularTemplatesAsync(int count, bool trackChanges);
    Task<IEnumerable<Template>> GetRecentTemplatesAsync(int count, bool trackChanges);
    Task<PagedList<Template>> SearchTemplatesAsync(TemplateParameters templateParameters, string searchTerm, bool trackChanges);
    Task<Template> GetTemplateByIdAsync(Guid templateId, bool trackChanges);
    void CreateTemplate(Template template);
    void UpdateTemplate(Template template);
    void DeleteTemplate(Template template);
}