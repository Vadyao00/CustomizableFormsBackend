using Contracts.IServices;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CustomizableForms.Application.Commands.RolesCommands;
using CustomizableForms.Application.Commands.UsersCommands;
using CustomizableForms.Application.Queries.UsersQueries;
using MediatR;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/admin")]
[ApiController]
[Authorize(Roles = "Admin", Policy = "NotBlockedUserPolicy")]
public class AdminController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery]UserParameters userParameters)
    {
        var baseResult = await sender.Send(new GetAllUsersQuery(userParameters));
        if (!baseResult.Success)
            return ProccessError(baseResult);

        var (users, metaData) = baseResult.GetResult<(IEnumerable<UserDto>, MetaData)>();

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        
        return Ok(users);
    }

    [HttpPost("users/{userId}/block")]
    public async Task<IActionResult> BlockUser(Guid userId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var baseResult = await sender.Send(new BlockUserCommand(userId, currentUser));
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return NoContent();
    }

    [HttpPost("users/{userId}/unblock")]
    public async Task<IActionResult> UnblockUser(Guid userId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var baseResult = await sender.Send(new UnblockUserCommand(userId));
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return NoContent();
    }

    [HttpPost("users/{userId}/add-admin")]
    public async Task<IActionResult> AddAdminRole(Guid userId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var baseResult = await sender.Send(new AssignRoleToUserCommand(userId, "Admin"));
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return NoContent();
    }

    [HttpPost("users/{userId}/remove-admin")]
    public async Task<IActionResult> RemoveAdminRole(Guid userId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var baseResult = await sender.Send(new RemoveRoleFromUserCommand(userId, "Admin"));
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return NoContent();
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var baseResult = await sender.Send(new DeleteUserCommand(userId));
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return NoContent();
    }
}