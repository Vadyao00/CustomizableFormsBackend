using Contracts.IServices;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/tags")]
[ApiController]
public class TagController(IServiceManager serviceManager, IHttpContextAccessor accessor)
    : ApiControllerBase(serviceManager, accessor)
{
    [HttpGet]
    public async Task<IActionResult> GetAllTags()
    {
        var result = await _serviceManager.TagService.GetAllTagsAsync();
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<TagDto>>());
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTags([FromQuery] string searchTerm)
    {
        var result = await _serviceManager.TagService.SearchTagsAsync(searchTerm);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<TagDto>>());
    }

    [HttpGet("cloud")]
    public async Task<IActionResult> GetTagCloud()
    {
        var result = await _serviceManager.TagService.GetTagCloudAsync();
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<TagCloudItemDto>>());
    }

    [HttpGet("{tagName}/templates")]
    public async Task<IActionResult> GetTemplatesByTag(string tagName)
    {
        var currentUser = await GetCurrentUserAsync();
        
        var result = await _serviceManager.TagService.GetTemplatesByTagAsync(tagName, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<TemplateDto>>());
    }
}