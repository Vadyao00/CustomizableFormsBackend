using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface ITagService
{
    Task<ApiBaseResponse> GetAllTagsAsync();
    Task<ApiBaseResponse> SearchTagsAsync(string searchTerm);
    Task<ApiBaseResponse> GetTagCloudAsync();
    Task<ApiBaseResponse> GetTemplatesByTagAsync(string tagName);
}