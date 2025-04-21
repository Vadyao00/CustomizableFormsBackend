using Contracts.IRepositories;
using CustomizableForms.Application.Commands.LikesCommands;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.LikesHandlers;

public class LikeTemplateHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<LikeTemplateCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(LikeTemplateCommand request, CancellationToken cancellationToken)
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
                return new ApiBadRequestResponse("You do not have permission to like this template");
            }

            var hasLiked = await repository.Like.HasUserLikedTemplateAsync(request.CurrentUser.Id, request.TemplateId, trackChanges: false);
            if (hasLiked)
            {
                return new ApiBadRequestResponse("You have already liked this template");
            }

            var like = new TemplateLike
            {
                Id = Guid.NewGuid(),
                TemplateId = request.TemplateId,
                UserId = request.CurrentUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            repository.Like.CreateLike(like);
            await repository.SaveAsync();
            
            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(LikeTemplateHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error liking template: {ex.Message}");
        }
    }
}