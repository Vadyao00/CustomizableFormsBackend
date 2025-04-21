using System.Security.Claims;
using Contracts.IServices;
using CustomizableForms.Application.Queries.UsersQueries;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Requirements;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace CustomizableForms.Controllers.Filters;

public class NotBlockedUserRequirementHandler(IServiceManager serviceManager, ISender sender)
    : AuthorizationHandler<NotBlockedUserRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotBlockedUserRequirement requirement)
    {
        var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userEmail))
            return;

        var baseResponse = await sender.Send(new GetUserByEmailQuery(userEmail));
        var user = baseResponse.GetResult<User>();
        if (user != null && user.IsActive)
        {
            context.Succeed(requirement);
        }
    }
}