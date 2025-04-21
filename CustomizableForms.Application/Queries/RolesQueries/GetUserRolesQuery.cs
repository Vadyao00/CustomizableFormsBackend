using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.RolesQueries;

public sealed record GetUserRolesQuery(Guid UserId) : IRequest<ApiBaseResponse>;