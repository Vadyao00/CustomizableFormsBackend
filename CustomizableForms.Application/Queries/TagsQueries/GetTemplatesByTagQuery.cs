using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TagsQueries;

public sealed record GetTemplatesByTagQuery(TemplateParameters TemplateParameters, string TagName, User CurrentUser) : IRequest<ApiBaseResponse>;