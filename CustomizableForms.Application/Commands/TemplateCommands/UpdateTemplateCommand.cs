using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.TemplateCommands;

public sealed record UpdateTemplateCommand(Guid TemplateId, TemplateForUpdateDto TemplateDto, User CurrentUser) : IRequest<ApiBaseResponse>;