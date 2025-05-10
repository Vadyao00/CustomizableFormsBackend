using Contracts.IServices;
using CustomizableForms.Controllers.Extensions;
using CustomizableForms.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomizableForms.Application.Commands.SalesforceCommands;
using CustomizableForms.Application.Queries.SalesforceQueries;
using MediatR;

namespace CustomizableForms.Controllers.Controllers
{
    [Route("api/salesforce")]
    [ApiController]
    [Authorize(Policy = "NotBlockedUserPolicy")]
    public class SalesforceController : ApiControllerBase
    {
        private readonly ISender _sender;

        public SalesforceController(
            IServiceManager serviceManager, 
            IHttpContextAccessor httpContextAccessor,
            ISender sender) 
            : base(serviceManager, httpContextAccessor)
        {
            _sender = sender;
        }

        [HttpPost("create-profile")]
        public async Task<IActionResult> CreateSalesforceProfile([FromBody] SalesforceProfileDto profileData)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var baseResult = await _sender.Send(new CreateSalesforceProfileCommand(profileData, currentUser.Id));
            if (!baseResult.Success)
                return ProccessError(baseResult);

            return Ok();
        }

        [HttpGet("profile-status")]
        public async Task<IActionResult> GetProfileStatus()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var baseResult = await _sender.Send(new GetSalesforceProfileStatusQuery(currentUser.Id));
            if (!baseResult.Success)
                return ProccessError(baseResult);

            var status = baseResult.GetResult<SalesforceProfileStatusDto>();
            return Ok(status);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var baseResult = await _sender.Send(new GetSalesforceProfileDataQuery(currentUser.Id));
            if (!baseResult.Success)
                return ProccessError(baseResult);

            var profileData = baseResult.GetResult<SalesforceProfileDto>();
            return Ok(profileData);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] SalesforceProfileDto profileData)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var baseResult = await _sender.Send(new UpdateSalesforceProfileCommand(profileData, currentUser.Id));
            if (!baseResult.Success)
                return ProccessError(baseResult);

            return Ok();
        }
    }
}