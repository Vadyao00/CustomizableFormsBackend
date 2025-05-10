using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.SalesforceQueries;

public record GetSalesforceProfileDataQuery(Guid UserId) : IRequest<ApiBaseResponse>;