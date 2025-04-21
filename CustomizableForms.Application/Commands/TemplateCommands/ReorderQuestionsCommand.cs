using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.TemplateCommands;

public sealed record ReorderQuestionsCommand(Guid TemplateId, List<Guid> QuestionIds, User CurrentUser) : IRequest<ApiBaseResponse>;