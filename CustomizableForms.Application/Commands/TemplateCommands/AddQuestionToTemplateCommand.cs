using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.TemplateCommands;

public sealed record AddQuestionToTemplateCommand(Guid TemplateId, QuestionForCreationDto QuestionDto, User CurrentUser) : IRequest<ApiBaseResponse>;