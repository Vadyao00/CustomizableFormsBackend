using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.CommentsCommands;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using CustomizableForms.Persistance.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CustomizableForms.Application.Handlers.CommentsHandlers;

public sealed class AddCommentHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper,
    IHubContext<CommentsHub> hubContext)
    : IRequestHandler<AddCommentCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (!template.IsPublic && 
                template.CreatorId != request.CurrentUser.Id && 
                !template.AllowedUsers.Any(au => au.UserId == request.CurrentUser.Id) &&
                !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to comment on this template");
            }

            var comment = new TemplateComment
            {
                Id = Guid.NewGuid(),
                Content = request.CommentDto.Content,
                CreatedAt = DateTime.UtcNow,
                TemplateId = request.TemplateId,
                UserId = request.CurrentUser.Id
            };

            repository.Comment.CreateComment(comment);
            await repository.SaveAsync();

            var createdComment = await repository.Comment.GetCommentByIdAsync(comment.Id, trackChanges: false);
            var commentResultDto = mapper.Map<CommentDto>(createdComment);

            await hubContext.Clients.Group(request.TemplateId.ToString())
                .SendAsync("ReceiveComment", commentResultDto, cancellationToken);
            
            return new ApiOkResponse<CommentDto>(commentResultDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(AddCommentHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error adding comment: {ex.Message}");
        }
    }
}