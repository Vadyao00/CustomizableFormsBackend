using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.CommentsCommands;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using CustomizableForms.Persistance.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CustomizableForms.Application.Handlers.CommentsHandlers;

public sealed class UpdateCommentHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper,
    IHubContext<CommentsHub> hubContext)
    : IRequestHandler<UpdateCommentCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await repository.Comment.GetCommentByIdAsync(request.CommentId, trackChanges: true);
            if (comment == null)
            {
                return new ApiBadRequestResponse("Comment not found");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (comment.UserId != request.CurrentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to update this comment");
            }

            comment.Content = request.CommentDto.Content;
            
            repository.Comment.UpdateComment(comment);
            await repository.SaveAsync();
            
            var updatedComment = await repository.Comment.GetCommentByIdAsync(request.CommentId, trackChanges: false);
            var commentResultDto = mapper.Map<CommentDto>(updatedComment);

            await hubContext.Clients.Group(comment.TemplateId.ToString())
                .SendAsync("UpdateComment", commentResultDto, cancellationToken);
            
            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(UpdateCommentHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error updating comment: {ex.Message}");
        }
    }
}