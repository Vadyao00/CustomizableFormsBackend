using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface ICommentService
{
    Task<ApiBaseResponse> GetTemplateCommentsAsync(Guid templateId);
    Task<ApiBaseResponse> AddCommentAsync(Guid templateId, CommentForCreationDto commentDto, User currentUser);
    Task<ApiBaseResponse> UpdateCommentAsync(Guid commentId, CommentForUpdateDto commentDto, User currentUser);
    Task<ApiBaseResponse> DeleteCommentAsync(Guid commentId, User currentUser);
}