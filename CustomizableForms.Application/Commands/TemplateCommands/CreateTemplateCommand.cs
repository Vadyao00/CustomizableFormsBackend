using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.TemplateCommands;

public sealed record CreateTemplateCommand(TemplateForCreationDto TemplateDto, User CurrentUser) : IRequest<ApiBaseResponse>;