using Contracts.IRepositories;
using CustomizableForms.Application.Queries.LikesQueries;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.LikesHandlers;

public class GetTemplateLikesCountHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<GetTemplateLikesCountQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetTemplateLikesCountQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            var likesCount = await repository.Like.GetTemplateLikesCountAsync(request.TemplateId);
            
            return new ApiOkResponse<int>(likesCount);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetTemplateLikesCountHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error getting template likes count: {ex.Message}");
        }
    }
}