using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record SearchTemplatesQuery(string SearchTerm) : IRequest<ApiBaseResponse>;