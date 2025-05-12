using Contracts.IServices;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/support")]
[ApiController]
public class SupportController : ApiControllerBase
{
    public SupportController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor)
        : base(serviceManager, httpContextAccessor)
    {
    }

    [HttpPost("ticket")]
    public async Task<IActionResult> CreateSupportTicket([FromBody] SupportTicketDto ticketDto)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            return Unauthorized();
        }

        var result = await _serviceManager.SupportService.CreateSupportTicketAsync(ticketDto, user.Name);
        
        if (!result.Success)
            return ProccessError(result);
        
        return Ok(new { message = "Support ticket created successfully" });
    }
}