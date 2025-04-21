using System.Text.Json;
using Contracts.IServices;
using CustomizableForms.Application.Queries.TagsQueries;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/tags")]
[ApiController]
public class TagController(IServiceManager serviceManager, IHttpContextAccessor accessor, ISender sender)
    : ApiControllerBase(serviceManager, accessor)
{
    [HttpGet]
    public async Task<IActionResult> GetAllTags()
    {
        var result = await sender.Send(new GetAllTagsQuery());
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<TagDto>>());
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTags([FromQuery] string searchTerm)
    {
        var result = await sender.Send(new SearchTagsQuery(searchTerm));
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<TagDto>>());
    }

    [HttpGet("cloud")]
    public async Task<IActionResult> GetTagCloud()
    {
        var result = await sender.Send(new GetTagCloudQuery());
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<TagCloudItemDto>>());
    }

    [HttpGet("{tagName}/templates")]
    public async Task<IActionResult> GetTemplatesByTag([FromQuery]TemplateParameters templateParameters, string tagName)
    {
        var currentUser = await GetCurrentUserAsync();
        
        var result = await sender.Send(new GetTemplatesByTagQuery(templateParameters, tagName, currentUser));
        if (!result.Success)
            return ProccessError(result);

        var (templates, metaData) = result.GetResult<(IEnumerable<TemplateDto>, MetaData)>();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        
        return Ok(templates);
    }
}