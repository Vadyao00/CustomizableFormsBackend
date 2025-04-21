using CustomizableForms.Domain.Entities;
using MediatR;

namespace CustomizableForms.Application.Queries.UsersQueries;

public sealed record GetUserByIdFromTokenQuery(Guid UserId) : IRequest<User?>;