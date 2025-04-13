using Contracts.IServices;
using CustomizableForms.Controllers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/templates/{templateId}/likes")]
[ApiController]
public class LikeController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor)
    : ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpGet("count")]
    public async Task<IActionResult> GetLikesCount(Guid templateId)
    {
        var result = await _serviceManager.LikeService.GetTemplateLikesCountAsync(templateId);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<int>());
    }

    [Authorize]
    [HttpGet("status")]
    public async Task<IActionResult> GetLikeStatus(Guid templateId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.LikeService.HasUserLikedTemplateAsync(templateId, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<bool>());
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LikeTemplate(Guid templateId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.LikeService.LikeTemplateAsync(templateId, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Ok();
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> UnlikeTemplate(Guid templateId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.LikeService.UnlikeTemplateAsync(templateId, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Ok();
    }
}