using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record GetTemplateQuestionsQuery(Guid TemplateId, User CurrentUser) : IRequest<ApiBaseResponse>;