using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface IFormService
{
    Task<ApiBaseResponse> GetUserFormsAsync(FormParameters formParameters, User currentUser);
    Task<ApiBaseResponse> GetTemplateFormsAsync(FormParameters formParameters, Guid templateId, User currentUser);
    Task<ApiBaseResponse> GetFormByIdAsync(Guid formId, User currentUser);
    Task<ApiBaseResponse> SubmitFormAsync(Guid templateId, FormForSubmissionDto formDto, User currentUser);
    Task<ApiBaseResponse> UpdateFormAsync(Guid formId, FormForUpdateDto formDto, User currentUser);
    Task<ApiBaseResponse> DeleteFormAsync(Guid formId, User currentUser);
    Task<ApiBaseResponse> GetFormResultsAggregationAsync(Guid templateId, User currentUser);
}