using Contracts.IServices;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Controllers.Filters;
using CustomizableForms.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/templates/{templateId}/comments")]
[ApiController]
public class CommentController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor)
    : ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpGet]
    public async Task<IActionResult> GetComments(Guid templateId)
    {
        var result = await _serviceManager.CommentService.GetTemplateCommentsAsync(templateId);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<CommentDto>>());
    }

    [Authorize]
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> AddComment(Guid templateId, [FromBody] CommentForCreationDto commentDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.CommentService.AddCommentAsync(templateId, commentDto, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<CommentDto>());
    }

    [Authorize]
    [HttpPut("{commentId}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] CommentForUpdateDto commentDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.CommentService.UpdateCommentAsync(commentId, commentDto, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await _serviceManager.CommentService.DeleteCommentAsync(commentId, currentUser);
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }
}