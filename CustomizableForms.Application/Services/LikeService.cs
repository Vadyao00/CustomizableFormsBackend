using Contracts.IRepositories;
using Contracts.IServices;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using CustomizableForms.Persistance.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CustomizableForms.Application.Services;

public class LikeService(IRepositoryManager repository, ILoggerManager logger, IHubContext<CommentsHub> hubContext) : ILikeService
{
    public async Task<ApiBaseResponse> LikeTemplateAsync(Guid templateId, User currentUser)
    {
        try
        {
            if (currentUser == null)
            {
                return new ApiBadRequestResponse("User not found");
            }

            var template = await repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (!template.IsPublic && 
                template.CreatorId != currentUser.Id && 
                !template.AllowedUsers.Any(au => au.UserId == currentUser.Id) &&
                !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to like this template");
            }

            var hasLiked = await repository.Like.HasUserLikedTemplateAsync(currentUser.Id, templateId, trackChanges: false);
            if (hasLiked)
            {
                return new ApiBadRequestResponse("You have already liked this template");
            }

            var like = new TemplateLike
            {
                Id = Guid.NewGuid(),
                TemplateId = templateId,
                UserId = currentUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            repository.Like.CreateLike(like);
            await repository.SaveAsync();

            var likesCount = await repository.Like.GetTemplateLikesCountAsync(templateId);

            await hubContext.Clients.Group(templateId.ToString())
                .SendAsync("UpdateLikes", likesCount);
            
            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(LikeTemplateAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error liking template: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> UnlikeTemplateAsync(Guid templateId, User currentUser)
    {
        try
        {
            if (currentUser == null)
            {
                return new ApiBadRequestResponse("User not found");
            }

            var template = await repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            var like = template.Likes?.FirstOrDefault(l => l.UserId == currentUser.Id);
            if (like == null)
            {
                return new ApiBadRequestResponse("You have not liked this template");
            }

            repository.Like.DeleteLike(like);
            await repository.SaveAsync();

            var likesCount = await repository.Like.GetTemplateLikesCountAsync(templateId);
                
            await hubContext.Clients.Group(templateId.ToString())
                .SendAsync("UpdateLikes", likesCount);
            
            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(UnlikeTemplateAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error unliking template: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetTemplateLikesCountAsync(Guid templateId)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            var likesCount = await repository.Like.GetTemplateLikesCountAsync(templateId);
            
            return new ApiOkResponse<int>(likesCount);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetTemplateLikesCountAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error getting template likes count: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> HasUserLikedTemplateAsync(Guid templateId, User currentUser)
    {
        try
        {
            if (currentUser == null)
            {
                return new ApiBadRequestResponse("User not found");
            }

            var template = await repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            var hasLiked = await repository.Like.HasUserLikedTemplateAsync(currentUser.Id, templateId, trackChanges: false);
            
            return new ApiOkResponse<bool>(hasLiked);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(HasUserLikedTemplateAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error checking if user has liked template: {ex.Message}");
        }
    }
}