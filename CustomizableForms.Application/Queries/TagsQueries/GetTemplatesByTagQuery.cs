using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TagsQueries;

public sealed record GetTemplatesByTagQuery(string TagName, User CurrentUser) : IRequest<ApiBaseResponse>;