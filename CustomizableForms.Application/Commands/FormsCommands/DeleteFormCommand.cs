using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.FormsCommands;

public sealed record DeleteFormCommand(Guid FormId, User CurrentUser) : IRequest<ApiBaseResponse>;