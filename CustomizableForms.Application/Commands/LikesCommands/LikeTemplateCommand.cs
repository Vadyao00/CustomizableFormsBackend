using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.LikesCommands;

public sealed record LikeTemplateCommand(Guid TemplateId, User CurrentUser) : IRequest<ApiBaseResponse>;