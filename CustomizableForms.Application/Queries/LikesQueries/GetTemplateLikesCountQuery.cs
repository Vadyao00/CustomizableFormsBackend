using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.LikesQueries;

public sealed record GetTemplateLikesCountQuery(Guid TemplateId) : IRequest<ApiBaseResponse>;