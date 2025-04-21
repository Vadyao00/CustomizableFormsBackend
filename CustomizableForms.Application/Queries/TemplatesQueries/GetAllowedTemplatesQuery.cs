using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record GetAllowedTemplatesQuery(TemplateParameters TemplateParameters, User CurrentUser) : IRequest<ApiBaseResponse>;