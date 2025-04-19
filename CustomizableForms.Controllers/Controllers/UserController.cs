using Contracts.IServices;
using CustomizableForms.Controllers.Filters;
using CustomizableForms.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/user")]
[ApiController]
[Authorize]
public class UserController( IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor)
    : ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UserPreferences userPreferences)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return Unauthorized();

        await serviceManager.UserService.UpdateUIUserAsync(userPreferences.PrefTheme, userPreferences.PrefLang, currentUser);
            
        return Ok();
    }
}