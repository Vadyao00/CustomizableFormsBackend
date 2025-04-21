using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.UsersQueries;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<ApiBaseResponse>;