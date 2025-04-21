using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record SearchTemplatesQuery(TemplateParameters TemplateParameters, string SearchTerm) : IRequest<ApiBaseResponse>;