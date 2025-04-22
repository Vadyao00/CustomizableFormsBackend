using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record GetUserPrivateTemplatesQuery(TemplateParameters TemplateParameters, Guid UserId, User CurrentUser) : IRequest<ApiBaseResponse>;