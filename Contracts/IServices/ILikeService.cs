using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface ILikeService
{
    Task<ApiBaseResponse> LikeTemplateAsync(Guid templateId, User currentUser);
    Task<ApiBaseResponse> UnlikeTemplateAsync(Guid templateId, User currentUser);
    Task<ApiBaseResponse> GetTemplateLikesCountAsync(Guid templateId);
    Task<ApiBaseResponse> HasUserLikedTemplateAsync(Guid templateId, User currentUser);
}