using System.Text.Json;
using Contracts.IServices;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Controllers.Filters;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/forms")]
[ApiController]
[Authorize(Policy = "NotBlockedUserPolicy")]
public class FormController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor)
    : ApiControllerBase(serviceManager, httpContextAccessor)
{ 
    [HttpGet("my")]
    public async Task<IActionResult> GetMyForms([FromQuery]FormParameters formParameters)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.FormService.GetUserFormsAsync(formParameters, currentUser);
        if (!result.Success)
            return ProccessError(result);

        var (forms, metaData) = result.GetResult<(IEnumerable<FormDto>, MetaData)>();

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        
        return Ok(forms);
    }

    [HttpGet("template/{templateId}")]
    public async Task<IActionResult> GetTemplateForms(Guid templateId, [FromQuery]FormParameters formParameters)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.FormService.GetTemplateFormsAsync(formParameters, templateId, currentUser);
        if (!result.Success)
            return ProccessError(result);

        var (forms, metaData) = result.GetResult<(IEnumerable<FormDto>, MetaData)>();

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        
        return Ok(forms);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetForm(Guid id)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.FormService.GetFormByIdAsync(id, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<FormDto>());
    }

    [HttpPost("template/{templateId}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> SubmitForm(Guid templateId, [FromBody] FormForSubmissionDto formDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.FormService.SubmitFormAsync(templateId, formDto, currentUser);
        if (!result.Success)
            return ProccessError(result);

        var form = result.GetResult<FormDto>();
        return CreatedAtAction(nameof(GetForm), new { id = form.Id }, form);
    }

    [HttpPut("{id}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateForm(Guid id, [FromBody] FormForUpdateDto formDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.FormService.UpdateFormAsync(id, formDto, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteForm(Guid id)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.FormService.DeleteFormAsync(id, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }

    [HttpGet("template/{templateId}/results")]
    public async Task<IActionResult> GetFormResults(Guid templateId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.FormService.GetFormResultsAggregationAsync(templateId, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<FormResultsAggregationDto>());
    }
}