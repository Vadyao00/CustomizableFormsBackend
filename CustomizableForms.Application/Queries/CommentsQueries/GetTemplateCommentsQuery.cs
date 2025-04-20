using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.CommentsQueries;

public sealed record GetTemplateCommentsQuery(Guid TemplateId) : IRequest<ApiBaseResponse>;