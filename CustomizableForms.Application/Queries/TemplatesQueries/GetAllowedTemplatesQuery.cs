using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.TemplatesQueries;

public sealed record GetAllowedTemplatesQuery(User CurrentUser) : IRequest<ApiBaseResponse>;