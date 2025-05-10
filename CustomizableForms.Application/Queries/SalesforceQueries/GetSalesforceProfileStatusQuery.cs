using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.SalesforceQueries;

public record GetSalesforceProfileStatusQuery(Guid UserId) : IRequest<ApiBaseResponse>;