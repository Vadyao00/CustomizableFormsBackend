﻿using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface IFormService
{
    Task<ApiBaseResponse> GetUserFormsAsync(User currentUser);
    Task<ApiBaseResponse> GetTemplateFormsAsync(Guid templateId, User currentUser);
    Task<ApiBaseResponse> GetFormByIdAsync(Guid formId, User currentUser);
    Task<ApiBaseResponse> SubmitFormAsync(Guid templateId, FormForSubmissionDto formDto, User currentUser);
    Task<ApiBaseResponse> UpdateFormAsync(Guid formId, FormForUpdateDto formDto, User currentUser);
    Task<ApiBaseResponse> DeleteFormAsync(Guid formId, User currentUser);
    Task<ApiBaseResponse> GetFormResultsAggregationAsync(Guid templateId, User currentUser);
}