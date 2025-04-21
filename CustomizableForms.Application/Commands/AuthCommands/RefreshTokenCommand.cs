using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.AuthCommands;

public sealed record RefreshTokenCommand(TokenDto TokenDto) : IRequest<ApiBaseResponse>;