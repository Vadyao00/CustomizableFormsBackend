using Contracts.IRepositories;
using CustomizableForms.Application.Commands.LikesCommands;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.LikesHandlers;

public class UnlikeTemplateHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<UnlikeTemplateCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(UnlikeTemplateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            var like = template.Likes?.FirstOrDefault(l => l.UserId == request.CurrentUser.Id);
            if (like == null)
            {
                return new ApiBadRequestResponse("You have not liked this template");
            }

            repository.Like.DeleteLike(like);
            await repository.SaveAsync();
            
            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(UnlikeTemplateHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error unliking template: {ex.Message}");
        }
    }
}