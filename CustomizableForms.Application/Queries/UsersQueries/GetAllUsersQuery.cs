using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.UsersQueries;

public sealed record GetAllUsersQuery(UserParameters UserParameters) : IRequest<ApiBaseResponse>;