using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.CommentsCommands;

public sealed record DeleteCommentCommand(Guid CommentId, User CurrentUser) : IRequest<ApiBaseResponse>;