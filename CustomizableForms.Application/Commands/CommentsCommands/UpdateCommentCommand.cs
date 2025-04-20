using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.CommentsCommands;

public sealed record UpdateCommentCommand(Guid CommentId, CommentForUpdateDto CommentDto, User CurrentUser) : IRequest<ApiBaseResponse>;