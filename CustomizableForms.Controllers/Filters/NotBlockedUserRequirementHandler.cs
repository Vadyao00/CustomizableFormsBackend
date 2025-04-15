using System.Security.Claims;
using Contracts.IServices;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CustomizableForms.Controllers.Filters;

public class NotBlockedUserRequirementHandler(IServiceManager serviceManager)
    : AuthorizationHandler<NotBlockedUserRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotBlockedUserRequirement requirement)
    {
        var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userEmail))
            return;

        var baseResponse = await serviceManager.UserService.GetUserByEmailWithoutCurrentUserAsync(userEmail);
        var user = baseResponse.GetResult<User>();
        if (user != null && user.IsActive)
        {
            context.Succeed(requirement);
        }
    }
}