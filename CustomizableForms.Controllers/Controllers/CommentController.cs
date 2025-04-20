using Contracts.IServices;
using CustomizableForms.Application.Commands.CommentsCommands;
using CustomizableForms.Application.Queries.CommentsQueries;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Controllers.Filters;
using CustomizableForms.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/templates/{templateId}/comments")]
[ApiController]
public class CommentController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpGet]
    public async Task<IActionResult> GetComments(Guid templateId)
    {
        var result = await sender.Send(new GetTemplateCommentsQuery(templateId));
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<IEnumerable<CommentDto>>());
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> AddComment(Guid templateId, [FromBody] CommentForCreationDto commentDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await sender.Send(new AddCommentCommand(templateId, commentDto, currentUser));
        if (!result.Success)
            return ProccessError(result);

        return Ok(result.GetResult<CommentDto>());
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpPut("{commentId}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] CommentForUpdateDto commentDto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await sender.Send(new UpdateCommentCommand(commentId, commentDto, currentUser));
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }

    [Authorize(Policy = "NotBlockedUserPolicy")]
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var result = await sender.Send(new DeleteCommentCommand(commentId, currentUser));
        if (!result.Success)
            return ProccessError(result);

        return NoContent();
    }
}