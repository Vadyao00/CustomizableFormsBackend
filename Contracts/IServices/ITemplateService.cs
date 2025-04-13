using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface ITemplateService
{
    Task<ApiBaseResponse> GetAllTemplatesAsync();
    Task<ApiBaseResponse> GetPublicTemplatesAsync();
    Task<ApiBaseResponse> GetTemplateByIdAsync(Guid templateId, User currentUser);
    Task<ApiBaseResponse> GetTemplateByIdWithoutTokenAsync(Guid templateId);
    Task<ApiBaseResponse> GetUserTemplatesAsync(Guid userId, User currentUser);
    Task<ApiBaseResponse> GetAccessibleTemplatesAsync(User currentUser);
    Task<ApiBaseResponse> GetPopularTemplatesAsync(int count);
    Task<ApiBaseResponse> GetRecentTemplatesAsync(int count);
    Task<ApiBaseResponse> SearchTemplatesAsync(string searchTerm);
    Task<ApiBaseResponse> CreateTemplateAsync(TemplateForCreationDto templateDto, User currentUser);
    Task<ApiBaseResponse> UpdateTemplateAsync(Guid templateId, TemplateForUpdateDto templateDto, User currentUser);
    Task<ApiBaseResponse> DeleteTemplateAsync(Guid templateId, User currentUser);
    Task<ApiBaseResponse> GetTemplateQuestionsAsync(Guid templateId, User currentUser);
    Task<ApiBaseResponse> GetTemplateQuestionsWithoutUserAsync(Guid templateId);
    Task<ApiBaseResponse> AddQuestionToTemplateAsync(Guid templateId, QuestionForCreationDto questionDto, User currentUser);
    Task<ApiBaseResponse> UpdateQuestionAsync(Guid templateId, Guid questionId, QuestionForUpdateDto questionDto, User currentUser);
    Task<ApiBaseResponse> DeleteQuestionAsync(Guid templateId, Guid questionId, User currentUser);
    Task<ApiBaseResponse> ReorderQuestionsAsync(Guid templateId, List<Guid> questionIds, User currentUser);
}