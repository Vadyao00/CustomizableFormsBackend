using Contracts.IServices;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Controllers.Filters;
using CustomizableForms.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/token")]
[ApiController]
public class TokenController( IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor)
    : ApiControllerBase(serviceManager, httpContextAccessor)
{
    [HttpPost("refresh")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
    {
        var tokenDtoToReturn = await serviceManager.AuthenticationService.RefreshToken(tokenDto);

        if (!tokenDtoToReturn.Success)
            return ProccessError(tokenDtoToReturn);
        
        var token = tokenDtoToReturn.GetResult<TokenDto>();
        
        return Ok(token);
    }
}