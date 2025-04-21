using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record GetTemplateByIdWithoutTokenQuery(Guid TemplateId) : IRequest<ApiBaseResponse>;