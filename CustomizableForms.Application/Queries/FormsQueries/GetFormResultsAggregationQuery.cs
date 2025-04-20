using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.FormsQueries;

public sealed record GetFormResultsAggregationQuery(Guid TemplateId, User CurrentUser) : IRequest<ApiBaseResponse>;