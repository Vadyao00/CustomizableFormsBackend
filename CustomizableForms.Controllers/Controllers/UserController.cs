using Contracts.IServices;
using CustomizableForms.Application.Commands.UsersCommands;
using CustomizableForms.Controllers.Filters;
using CustomizableForms.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/user")]
[ApiController]
[Authorize]
public class UserController( IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UserPreferences userPreferences)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        var baseResult = await sender.Send(new UpdateUIUserCommand(userPreferences.PrefTheme, userPreferences.PrefLang, currentUser));
        if (!baseResult.Success)
            return ProccessError(baseResult);
        return Ok();
    }
}