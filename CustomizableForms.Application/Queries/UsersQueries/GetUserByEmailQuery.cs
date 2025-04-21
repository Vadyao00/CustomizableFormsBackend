using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.UsersQueries;

public sealed record GetUserByEmailQuery(string Email) : IRequest<ApiBaseResponse>;