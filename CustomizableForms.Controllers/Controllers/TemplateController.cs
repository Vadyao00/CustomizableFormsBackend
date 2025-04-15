using Contracts.IServices;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Controllers.Filters;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/templates")]
[ApiController]
public class TemplateController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor) :
    ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpGet]
    public async Task<IActionResult> GetTemplates()
    {
        var baseResult = await _serviceManager.TemplateService.GetPublicTemplatesAsync();
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return Ok(baseResult.GetResult<IEnumerable<TemplateDto>>());
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTemplates([FromQuery] string searchTerm)
    {
        var baseResult = await _serviceManager.TemplateService.SearchTemplatesAsync(searchTerm);
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return Ok(baseResult.GetResult<IEnumerable<TemplateDto>>());
    }

    [HttpGet("popular/{count}")]
    public async Task<IActionResult> GetPopularTemplates(int count)
    {
        var baseResult = await _serviceManager.TemplateService.GetPopularTemplatesAsync(count);
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return Ok(baseResult.GetResult<IEnumerable<TemplateDto>>());
    }

    [HttpGet("recent/{count}")]
    public async Task<IActionResult> GetRecentTemplates(int count)
    {
        var baseResult = await _serviceManager.TemplateService.GetRecentTemplatesAsync(count);
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return Ok(baseResult.GetResult<IEnumerable<TemplateDto>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTemplate(Guid id)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser is null)
        {
            var baseResult = await _serviceManager.TemplateService.GetTemplateByIdWithoutTokenAsync(id);
            if (!baseResult.Success)
                return ProccessError(baseResult);

            return Ok(baseResult.GetResult<TemplateDto>());
        }

        var result = await _serviceManager.TemplateService.GetTemplateByIdAsync(id, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<TemplateDto>());
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyTemplates()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.TemplateService.GetUserTemplatesAsync(currentUser.Id, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<TemplateDto>>());
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpGet("accessible")]
    public async Task<IActionResult> GetAccessibleTemplates()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.TemplateService.GetAccessibleTemplatesAsync(currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<TemplateDto>>());
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateTemplate([FromBody] TemplateForCreationDto templateDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.TemplateService.CreateTemplateAsync(templateDto, currentUser);
        if (!result.Success)
            return ProccessError(result);

        var template = result.GetResult<TemplateDto>();
        return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpPut("{id}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] TemplateForUpdateDto templateDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.TemplateService.UpdateTemplateAsync(id, templateDto, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(Guid id)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.TemplateService.DeleteTemplateAsync(id, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }

    [HttpGet("{id}/questions")]
    public async Task<IActionResult> GetTemplateQuestions(Guid id)
    {
        var currentUser = await GetCurrentUserAsync();
        ApiBaseResponse result;
        if (currentUser is not null)
            result = await _serviceManager.TemplateService.GetTemplateQuestionsAsync(id, currentUser);
        else
            result = await _serviceManager.TemplateService.GetTemplateQuestionsWithoutUserAsync(id);
        
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<QuestionDto>>());
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpPost("{id}/questions")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> AddQuestion(Guid id, [FromBody] QuestionForCreationDto questionDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.TemplateService.AddQuestionToTemplateAsync(id, questionDto, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Created($"/api/templates/{id}/questions", result.GetResult<QuestionDto>());
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpPut("{templateId}/questions/{questionId}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateQuestion(Guid templateId, Guid questionId, [FromBody] QuestionForUpdateDto questionDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.TemplateService.UpdateQuestionAsync(templateId, questionId, questionDto, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpDelete("{templateId}/questions/{questionId}")]
    public async Task<IActionResult> DeleteQuestion(Guid templateId, Guid questionId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.TemplateService.DeleteQuestionAsync(templateId, questionId, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpPost("{id}/questions/reorder")]
    public async Task<IActionResult> ReorderQuestions(Guid id, [FromBody] List<Guid> questionIds)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.TemplateService.ReorderQuestionsAsync(id, questionIds, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }
}