using Contracts.IServices;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/admin")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor)
    : ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var baseResult = await _serviceManager.UserService.GetAllUsersAsync(currentUser);
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return Ok(baseResult.GetResult<IEnumerable<UserDto>>());
    }

    [HttpPost("users/{userId}/block")]
    public async Task<IActionResult> BlockUser(Guid userId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var baseResult = await _serviceManager.UserService.BlockUserAsync(userId, currentUser);
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

        var baseResult = await _serviceManager.UserService.UnblockUserAsync(userId, currentUser);
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

        var baseResult = await _serviceManager.RoleService.AssignRoleToUserAsync(userId, "Admin", currentUser);
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

        var baseResult = await _serviceManager.RoleService.RemoveRoleFromUserAsync(userId, "Admin", currentUser);
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

        var baseResult = await _serviceManager.UserService.DeleteUserAsync(userId, currentUser);
        if (!baseResult.Success)
            return ProccessError(baseResult);

        return NoContent();
    }
}