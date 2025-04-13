using CustomizableForms.Domain.Entities;

namespace Contracts.IRepositories;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetAllTagsAsync(bool trackChanges);
    Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm, bool trackChanges);
    Task<Tag> GetTagByIdAsync(Guid tagId, bool trackChanges);
    Task<Tag> GetTagByNameAsync(string name, bool trackChanges);
    void CreateTag(Tag tag);
    void UpdateTag(Tag tag);
    void DeleteTag(Tag tag);
}