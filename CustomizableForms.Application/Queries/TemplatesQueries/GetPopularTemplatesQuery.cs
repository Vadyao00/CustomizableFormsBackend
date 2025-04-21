using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record GetPopularTemplatesQuery(int Count) : IRequest<ApiBaseResponse>;