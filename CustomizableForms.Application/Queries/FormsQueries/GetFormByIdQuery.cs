using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.FormsQueries;

public sealed record GetFormByIdQuery(Guid FormId, User CurrentUser) : IRequest<ApiBaseResponse>;