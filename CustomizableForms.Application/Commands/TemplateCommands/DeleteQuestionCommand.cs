using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.TemplateCommands;

public sealed record DeleteQuestionCommand(Guid TemplateId, Guid QuestionId, User CurrentUser) : IRequest<ApiBaseResponse>;