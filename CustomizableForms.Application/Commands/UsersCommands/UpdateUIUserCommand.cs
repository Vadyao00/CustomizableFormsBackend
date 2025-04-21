using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.UsersCommands;

public sealed record UpdateUIUserCommand(string PrefTheme, string PrefLang, User CurrentUser) : IRequest<ApiBaseResponse>;