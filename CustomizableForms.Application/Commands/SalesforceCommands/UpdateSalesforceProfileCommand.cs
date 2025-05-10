using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.SalesforceCommands;

public record UpdateSalesforceProfileCommand(SalesforceProfileDto ProfileData, Guid UserId) : IRequest<ApiBaseResponse>;