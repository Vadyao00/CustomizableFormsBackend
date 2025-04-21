using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TagsQueries;

public sealed record GetTagCloudQuery : IRequest<ApiBaseResponse>;