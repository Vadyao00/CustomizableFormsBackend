using Contracts.IRepositories;
using CustomizableForms.Application.Commands.LikesCommands;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.LikesHandlers;

public class HasUserLikedTemplateHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<HasUserLikedTemplateCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(HasUserLikedTemplateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            var hasLiked = await repository.Like.HasUserLikedTemplateAsync(request.CurrentUser.Id, request.TemplateId, trackChanges: false);
            
            return new ApiOkResponse<bool>(hasLiked);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(HasUserLikedTemplateHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error checking if user has liked template: {ex.Message}");
        }
    }
}