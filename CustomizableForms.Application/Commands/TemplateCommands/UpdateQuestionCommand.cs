using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.TemplateCommands;

public sealed record UpdateQuestionCommand(Guid TemplateId, Guid QuestionId, QuestionForUpdateDto QuestionDto, User CurrentUser) : IRequest<ApiBaseResponse>;