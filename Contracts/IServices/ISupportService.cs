using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;

namespace Contracts.IServices;

public interface ISupportService
{
    Task<ApiBaseResponse> CreateSupportTicketAsync(SupportTicketDto ticketDto, string username);
}