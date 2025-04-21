using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.TemplateCommands;

public sealed record DeleteTemplateCommand(Guid TemplateId, User CurrentUser) : IRequest<ApiBaseResponse>;