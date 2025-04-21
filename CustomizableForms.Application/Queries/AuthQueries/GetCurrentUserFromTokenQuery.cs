using CustomizableForms.Domain.Entities;
using MediatR;

namespace CustomizableForms.Application.Queries.AuthQueries;

public sealed record GetCurrentUserFromTokenQuery(string Token) : IRequest<User>;