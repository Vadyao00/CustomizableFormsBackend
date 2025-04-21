using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TagsQueries;

public sealed record SearchTagsQuery(string SearchTerm) : IRequest<ApiBaseResponse>;