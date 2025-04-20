using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.CommentsCommands;

public sealed record AddCommentCommand(Guid TemplateId, CommentForCreationDto CommentDto, User CurrentUser) : IRequest<ApiBaseResponse>;