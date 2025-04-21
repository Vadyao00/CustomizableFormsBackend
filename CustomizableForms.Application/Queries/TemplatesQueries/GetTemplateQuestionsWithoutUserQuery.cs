using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record GetTemplateQuestionsWithoutUserQuery(Guid TemplateId) : IRequest<ApiBaseResponse>;