using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record GetPublicTemplatesQuery(TemplateParameters TemplateParameters) : IRequest<ApiBaseResponse>;