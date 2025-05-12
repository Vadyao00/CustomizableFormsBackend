using System.Text.Json;
using Contracts.IServices;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;

namespace CustomizableForms.Application.Services;

public class SupportService : ISupportService
{
    private readonly ILoggerManager _logger;
    private readonly IDropboxService _dropboxService;

    public SupportService(ILoggerManager logger, IDropboxService dropboxService)
    {
        _logger = logger;
        _dropboxService = dropboxService;
    }

    public async Task<ApiBaseResponse> CreateSupportTicketAsync(SupportTicketDto ticketDto, string username)
    {
        try
        {
            var ticket = new
            {
                ReportedBy = username,
                Summary = ticketDto.Summary,
                Priority = ticketDto.Priority,
                Link = ticketDto.Link,
                CreatedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(ticket, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            var fileName = $"ticket_{Guid.NewGuid():N}.json";

            await _dropboxService.UploadJsonFileAsync(json, fileName);

            _logger.LogInfo($"Support ticket created by {username} with summary: {ticketDto.Summary}");
            
            return new ApiOkResponse<string>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating support ticket: {ex.Message}");
            return new ApiBadRequestResponse(ex.Message);
        }
    }
}