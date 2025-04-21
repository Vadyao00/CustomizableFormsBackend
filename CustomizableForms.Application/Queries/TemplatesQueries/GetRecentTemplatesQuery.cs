using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record GetRecentTemplatesQuery(int Count) : IRequest<ApiBaseResponse>;