using System.Text.Json;
using Contracts.IServices;
using CustomizableForms.Application.Commands.TemplateCommands;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Controllers.Filters;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/templates")]
[ApiController]
public class TemplateController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor, ISender sender) :
    ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpGet]
    public async Task<IActionResult> GetTemplates([FromQuery]TemplateParameters templateParameters)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser is null)
        {
            var baseResult = await sender.Send(new GetPublicTemplatesQuery(templateParameters));
            if (!baseResult.Success)
                return ProccessError(baseResult);
            
            var (templatess, metaDataa) = baseResult.GetResult<(IEnumerable<TemplateDto>, MetaData)>();
            
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaDataa));
            
            return Ok(templatess);
        }
        var result = await sender.Send(new GetAllowedTemplatesQuery(templateParameters, currentUser));
        if (!result.Success)
            return ProccessError(result);

        var (templates, metaData) = result.GetResult<(IEnumerable<TemplateDto>, MetaData)>();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        
        return Ok(templates);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTemplates([FromQuery] string searchTerm, [FromQuery]TemplateParameters templateParameters)
    {
        var baseResult = await sender.Send(new SearchTemplatesQuery(templateParameters, searchTerm));
        if (!baseResult.Success)
            return ProccessError(baseResult);

        var (templates, metaData) = baseResult.GetResult<(IEnumerable<TemplateDto>, MetaData)>();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        
        return Ok(templates);
    }

    [HttpGet("popular/{count}")]
    public async Task<IActionResult> GetPopularTemplates(int count)
    {
        var baseResult = await sender.Send(new GetPopularTemplatesQuery(count));
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return Ok(baseResult.GetResult<IEnumerable<TemplateDto>>());
    }

    [HttpGet("recent/{count}")]
    public async Task<IActionResult> GetRecentTemplates(int count)
    {
        var baseResult = await sender.Send(new GetRecentTemplatesQuery(count));
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
            var baseResult = await sender.Send(new GetTemplateByIdWithoutTokenQuery(id));
            if (!baseResult.Success)
                return ProccessError(baseResult);

            return Ok(baseResult.GetResult<TemplateDto>());
        }

        var result = await sender.Send(new GetTemplateByIdQuery(id,currentUser));
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<TemplateDto>());
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyTemplates([FromQuery]TemplateParameters templateParameters)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await sender.Send(new GetUserTemplatesQuery(templateParameters, currentUser.Id, currentUser));
        if (!result.Success)
            return ProccessError(result);

        var (templates, metaData) = result.GetResult<(IEnumerable<TemplateDto>, MetaData)>();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        
        return Ok(templates);
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateTemplate([FromBody] TemplateForCreationDto templateDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await sender.Send(new CreateTemplateCommand(templateDto, currentUser));
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

        var result = await sender.Send(new UpdateTemplateCommand(id, templateDto, currentUser));
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

        var result = await sender.Send(new DeleteTemplateCommand(id, currentUser));
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
            result = await sender.Send(new GetTemplateQuestionsQuery(id, currentUser));
        else
            result = await sender.Send(new GetTemplateQuestionsWithoutUserQuery(id));
        
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

        var result = await sender.Send(new AddQuestionToTemplateCommand(id, questionDto, currentUser));
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

        var result = await sender.Send(new UpdateQuestionCommand(templateId, questionId, questionDto, currentUser));
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

        var result = await sender.Send(new DeleteQuestionCommand(templateId, questionId, currentUser));
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

        var result = await sender.Send(new ReorderQuestionsCommand(id, questionIds, currentUser));
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }
}