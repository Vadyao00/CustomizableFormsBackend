using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Persistance.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class TemplateRepository : RepositoryBase<Template>, ITemplateRepository
{
    public TemplateRepository(CustomizableFormsContext context) : base(context)
    {
    }

    public async Task<PagedList<Template>> GetTemplatesByTagAsync(TemplateParameters templateParameters, string tagName, User currentUser, bool isAdmin, bool trackChanges)
    {
        var tag = await DbContext.Tags
            .FirstOrDefaultAsync(t => t.Name == tagName);
        
        if (tag == null)
        {
            return new PagedList<Template>(new List<Template>(), 0, templateParameters.PageNumber, templateParameters.PageSize);
        }

        IQueryable<Template> query;
        
        if (currentUser is not null)
        {
            query = FindByCondition(t => t.IsPublic || (t.CreatorId == currentUser.Id || t.AllowedUsers.Any(au => au.UserId == currentUser.Id) || isAdmin), trackChanges);
        }
        else
        {
            query = FindByCondition(t => t.IsPublic, trackChanges);
        }
        
        query = query.Where(t => t.TemplateTags.Any(tt => tt.TagId == tag.Id));
        
        query = query.Search(templateParameters.searchTopic);
        
        query = query.Sort(templateParameters.OrderBy);
        
        query = query
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .Include(t => t.Likes)
            .Include(t => t.Comments);
        
        var count = await query.CountAsync();
        
        var templates = await query
            .Skip((templateParameters.PageNumber - 1) * templateParameters.PageSize)
            .Take(templateParameters.PageSize)
            .ToListAsync();
        
        return new PagedList<Template>(templates, count, templateParameters.PageNumber, templateParameters.PageSize);
    }
    
    public async Task<PagedList<Template>> GetPublicTemplatesAsync(TemplateParameters templateParameters, bool trackChanges)
    {
        var templates = await FindByCondition(t => t.IsPublic, trackChanges)
            .Search(templateParameters.searchTopic)
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .Sort(templateParameters.OrderBy)
            .Skip((templateParameters.PageNumber - 1) * templateParameters.PageSize)
            .Take(templateParameters.PageSize)
            .ToListAsync();

        var count = await FindByCondition(t => t.IsPublic, trackChanges).Search(templateParameters.searchTopic)
            .CountAsync();

        return new PagedList<Template>(templates, count, templateParameters.PageNumber, templateParameters.PageSize);
    }
    
    public async Task<PagedList<Template>> GetAllowedTemplatesAsync(TemplateParameters templateParameters, User currentUser, bool isAdmin, bool trackChanges)
    {
        var templates = await FindByCondition(t => 
                t.IsPublic ||
                (currentUser != null &&
                    (t.CreatorId == currentUser.Id ||
                     t.AllowedUsers.Any(au => au.UserId == currentUser.Id) ||
                     isAdmin)),
                trackChanges)
            .Search(templateParameters.searchTopic)
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
            .ThenInclude(tt => tt.Tag)
            .Sort(templateParameters.OrderBy)
            .Skip((templateParameters.PageNumber - 1) * templateParameters.PageSize)
            .Take(templateParameters.PageSize)
            .ToListAsync();
        
        var count = await FindByCondition(t => 
                    t.IsPublic ||
                    (currentUser != null &&
                     (t.CreatorId == currentUser.Id ||
                      t.AllowedUsers.Any(au => au.UserId == currentUser.Id) ||
                      isAdmin)),
                trackChanges).
            Search(templateParameters.searchTopic)
            .CountAsync();

        return new PagedList<Template>(templates, count, templateParameters.PageNumber, templateParameters.PageSize);
    }

    public async Task<PagedList<Template>> GetUserTemplatesAsync(TemplateParameters templateParameters, Guid userId, bool trackChanges)
    {
        var templates =  await FindByCondition(t => t.CreatorId == userId, trackChanges)
            .Include(t => t.Creator)
            .Include(t => t.Forms)
            .Include(t => t.TemplateTags)
                .ThenInclude(tt => tt.Tag)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((templateParameters.PageNumber - 1) * templateParameters.PageSize)
            .Take(templateParameters.PageSize)
            .ToListAsync();

        var count = await FindByCondition(t => t.CreatorId == userId, trackChanges).CountAsync();

        return new PagedList<Template>(templates, count, templateParameters.PageNumber, templateParameters.PageSize);
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

    public async Task<PagedList<Template>> SearchTemplatesAsync(TemplateParameters templateParameters, string searchTerm, bool trackChanges)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new PagedList<Template>([], 0, 1, 5);

        searchTerm = searchTerm.Trim().ToLower().Replace("'", "''");
        
        var templates = await DbContext.Templates
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
            .Skip((templateParameters.PageNumber - 1) * templateParameters.PageSize)
            .Take(templateParameters.PageSize)
            .ToListAsync();

        var count = await DbContext.Templates
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
                ) DESC").CountAsync();
        
        return new PagedList<Template>(templates, count, templateParameters.PageNumber, templateParameters.PageSize);
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