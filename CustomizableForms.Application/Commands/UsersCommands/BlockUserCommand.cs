using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.UsersCommands;

public sealed record BlockUserCommand(Guid UserId, User CurrentUser) : IRequest<ApiBaseResponse>;