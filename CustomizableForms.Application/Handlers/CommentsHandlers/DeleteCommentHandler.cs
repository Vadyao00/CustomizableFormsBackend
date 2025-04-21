using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.CommentsCommands;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using CustomizableForms.Persistance.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CustomizableForms.Application.Handlers.CommentsHandlers;

public sealed class DeleteCommentHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IHubContext<CommentsHub> hubContext)
    : IRequestHandler<DeleteCommentCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
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

            if (comment.UserId != request.CurrentUser.Id && comment.Template.CreatorId != request.CurrentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to delete this comment");
            }

            var templateId = comment.TemplateId;
        
            repository.Comment.DeleteComment(comment);
            await repository.SaveAsync();

            await hubContext.Clients.Group(templateId.ToString())
                .SendAsync("DeleteComment", request.CommentId, cancellationToken);

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(DeleteCommentHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error deleting comment: {ex.Message}");
        }
    }
}