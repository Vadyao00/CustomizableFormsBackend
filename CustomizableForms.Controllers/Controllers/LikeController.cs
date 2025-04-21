using Contracts.IServices;
using CustomizableForms.Application.Commands.LikesCommands;
using CustomizableForms.Application.Queries.LikesQueries;
using CustomizableForms.Controllers.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/templates/{templateId}/likes")]
[ApiController]
public class LikeController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpGet("count")]
    public async Task<IActionResult> GetLikesCount(Guid templateId)
    {
        var result = await sender.Send(new GetTemplateLikesCountQuery(templateId));
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<int>());
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpGet("status")]
    public async Task<IActionResult> GetLikeStatus(Guid templateId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await sender.Send(new HasUserLikedTemplateCommand(templateId, currentUser));
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<bool>());
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpPost]
    public async Task<IActionResult> LikeTemplate(Guid templateId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await sender.Send(new LikeTemplateCommand(templateId, currentUser));
        if (!result.Success)
            return ProccessError(result);

        return Ok();
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpDelete]
    public async Task<IActionResult> UnlikeTemplate(Guid templateId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await sender.Send(new UnlikeTemplateCommand(templateId, currentUser));
        if (!result.Success)
            return ProccessError(result);

        return Ok();
    }
}