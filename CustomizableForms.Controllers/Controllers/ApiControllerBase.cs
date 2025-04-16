using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Contracts.IServices;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.ErrorModels;
using CustomizableForms.Domain.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

public class ApiControllerBase : Controller
{
    protected readonly IServiceManager _serviceManager;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    
    protected ApiControllerBase(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor)
    {
        _serviceManager = serviceManager;
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected async Task<User> GetCurrentUserAsync()
    {
        if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return null;
        }

        string authHeaderValue = authHeader.ToString();
        
        if (!authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        string token = authHeaderValue.Substring("Bearer ".Length).Trim();
        
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        try
        {
            return await _serviceManager.AuthenticationService.GetCurrentUserFromTokenAsync(token);
        }
        catch
        {
            return null;
        }
    }
    
    [HttpHead]
    public IActionResult ProccessError(ApiBaseResponse baseResponse)
    {
        return baseResponse switch
        {
            BadUserBadRequestResponse response => Unauthorized(new ErrorDetails
            {
                Message = response.Message,
                StatusCode = StatusCodes.Status401Unauthorized
            }),
            BlockedUserBadRequestResponse response => new ObjectResult(new ErrorDetails
            {
                Message = response.Message,
                StatusCode = StatusCodes.Status400BadRequest
            })
            {
                StatusCode = StatusCodes.Status400BadRequest
            },
            InvalidEmailBadRequestResponse response => Conflict(new ErrorDetails
            {
                Message = response.Message,
                StatusCode = StatusCodes.Status409Conflict
            }),
            RefreshTokenBadRequestResponse response => BadRequest(new ErrorDetails
            {
                Message = response.Message,
                StatusCode = StatusCodes.Status400BadRequest
            }),
            ApiBadRequestResponse response => BadRequest(new ErrorDetails
            {
                Message = response.Message,
                StatusCode = StatusCodes.Status400BadRequest
            }),
            _ => throw new NotImplementedException()
        };
    }
}