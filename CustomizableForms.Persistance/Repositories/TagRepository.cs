using Contracts.IRepositories;
using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance.Repositories;

public class TagRepository : RepositoryBase<Tag>, ITagRepository
{
    public TagRepository(CustomizableFormsContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Tag>> GetAllTagsAsync(bool trackChanges)
    {
        return await FindAll(trackChanges)
            .Include(t => t.TemplateTags)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm, bool trackChanges)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<Tag>();

        return await FindByCondition(t => EF.Functions.ILike(t.Name, $"{searchTerm}%"), trackChanges)
            .OrderBy(t => t.Name)
            .Take(10)
            .ToListAsync();
    }

    public async Task<Tag> GetTagByIdAsync(Guid tagId, bool trackChanges)
    {
        return await FindByCondition(t => t.Id == tagId, trackChanges)
            .FirstOrDefaultAsync();
    }

    public async Task<Tag> GetTagByNameAsync(string name, bool trackChanges)
    {
        return await FindByCondition(t => t.Name.ToLower() == name.ToLower(), trackChanges)
            .FirstOrDefaultAsync();
    }

    public void CreateTag(Tag tag) => Create(tag);

    public void UpdateTag(Tag tag) => Update(tag);

    public void DeleteTag(Tag tag) => Delete(tag);
}