using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class TemplateRepository : RepositoryBase<Template>, ITemplateRepository
{
    public TemplateRepository(CustomizableFormsContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Template>> GetAllTemplatesAsync(bool trackChanges)
    {
        return await FindAll(trackChanges)
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> GetPublicTemplatesAsync(bool trackChanges)
    {
        return await FindByCondition(t => t.IsPublic, trackChanges)
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> GetUserTemplatesAsync(Guid userId, bool trackChanges)
    {
        return await FindByCondition(t => t.CreatorId == userId, trackChanges)
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> GetAccessibleTemplatesAsync(Guid userId, bool trackChanges)
    {
        return await DbContext.Templates
            .Where(t => t.IsPublic || t.CreatorId == userId || t.AllowedUsers.Any(au => au.UserId == userId))
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .OrderByDescending(t => t.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> GetPopularTemplatesAsync(int count, bool trackChanges)
    {
        return await DbContext.Templates
            .Where(t => t.IsPublic)
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .OrderByDescending(t => t.Forms.Count)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> GetRecentTemplatesAsync(int count, bool trackChanges)
    {
        return await DbContext.Templates
            .Where(t => t.IsPublic)
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> SearchTemplatesAsync(string searchTerm, bool trackChanges)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<Template>();

        searchTerm = searchTerm.Trim().ToLower().Replace("'", "''");
        
        return await DbContext.Templates
            .FromSqlRaw($@"
                SELECT t.* FROM ""Templates"" t
                WHERE t.""IsPublic"" = true AND (
                    to_tsvector('russian', t.""Title"" || ' ' || COALESCE(t.""Description"", '')) @@ 
                    to_tsquery('russian', '{searchTerm.Replace(" ", "&")}:*')
                    OR EXISTS (
                        SELECT 1 FROM ""Questions"" q 
                        WHERE q.""TemplateId"" = t.""Id"" AND 
                        to_tsvector('russian', q.""Title"" || ' ' || COALESCE(q.""Description"", '')) @@ 
                        to_tsquery('russian', '{searchTerm.Replace(" ", "&")}:*')
                    )
                    OR EXISTS (
                        SELECT 1 FROM ""TemplateComments"" c 
                        WHERE c.""TemplateId"" = t.""Id"" AND 
                        to_tsvector('russian', c.""Content"") @@ 
                        to_tsquery('russian', '{searchTerm.Replace(" ", "&")}:*')
                    )
                    OR EXISTS (
                        SELECT 1 FROM ""TemplateTags"" tt
                        JOIN ""Tags"" tag ON tt.""TagId"" = tag.""Id""
                        WHERE tt.""TemplateId"" = t.""Id"" AND 
                        to_tsvector('russian', tag.""Name"") @@ 
                        to_tsquery('russian', '{searchTerm.Replace(" ", "&")}:*')
                    )
                )
                ORDER BY ts_rank(
                    to_tsvector('russian', t.""Title"" || ' ' || COALESCE(t.""Description"", '')), 
                    to_tsquery('russian', '{searchTerm.Replace(" ", "&")}:*')
                ) DESC")
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Template> GetTemplateByIdAsync(Guid templateId, bool trackChanges)
    {
        return await FindByCondition(t => t.Id == templateId, trackChanges)
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.Questions.OrderBy(q => q.OrderIndex))
            .Include(t => t.Likes)
            .Include(t => t.Comments)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .Include(t => t.AllowedUsers)
                .ThenInclude(au => au.User)
            .FirstOrDefaultAsync();
    }

    public void CreateTemplate(Template template) => Create(template);

    public void UpdateTemplate(Template template) => Update(template);

    public void DeleteTemplate(Template template) => Delete(template);
}