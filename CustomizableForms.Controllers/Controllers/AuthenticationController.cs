using Contracts.IServices;
using CustomizableForms.Controllers.Filters;
using CustomizableForms.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController(IServiceManager service, IHttpContextAccessor accessor)
    : ApiControllerBase(service, accessor)
{
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
    {
        var baseResult = await service.AuthenticationService.RegisterUser(userForRegistration);
        if(!baseResult.Success)
            return ProccessError(baseResult);
        
        return StatusCode(201);
    }
    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
    {
        var baseResult = await service.AuthenticationService.ValidateUser(user);
        if(!baseResult.Success)
            return ProccessError(baseResult);

        var tokenDto = await service.AuthenticationService.CreateToken(populateExp: true);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(1),
            Path = "/"
        };
        
        var context = accessor.HttpContext;
        context.Response.Cookies.Append("AccessToken", tokenDto.AccessToken, cookieOptions);
        context.Response.Cookies.Append("RefreshToken", tokenDto.RefreshToken, cookieOptions);

        return Ok(tokenDto);
    }
}