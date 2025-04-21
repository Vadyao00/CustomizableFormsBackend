using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.UsersCommands;

public sealed record UnblockUserCommand(Guid UserId) : IRequest<ApiBaseResponse>;