using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.RolesCommands;

public sealed record AssignRoleToUserCommand(Guid UserId, string RoleName) : IRequest<ApiBaseResponse>;