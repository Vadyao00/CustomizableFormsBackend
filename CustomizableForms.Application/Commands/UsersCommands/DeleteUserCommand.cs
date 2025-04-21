using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.UsersCommands;

public sealed record DeleteUserCommand(Guid Id) : IRequest<ApiBaseResponse>;