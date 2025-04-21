using CustomizableForms.Domain.DTOs;
using MediatR;

namespace CustomizableForms.Application.Commands.AuthCommands;

public sealed record CreateTokenCommand(bool PopulateExp) : IRequest<TokenDto>;