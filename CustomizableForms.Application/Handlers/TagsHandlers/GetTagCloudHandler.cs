using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TagsQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TagsHandlers;

public class GetTagCloudHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<GetTagCloudQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetTagCloudQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var tags = await repository.Tag.GetAllTagsAsync(trackChanges: false);
            
            var maxCount = tags.Max(t => t.TemplateTags?.Count ?? 0);
            var tagCloud = tags.Select(t => new TagCloudItemDto
            {
                Name = t.Name,
                Weight = maxCount > 0 
                    ? (int)Math.Ceiling((t.TemplateTags?.Count ?? 0) * 5.0 / maxCount) 
                    : 1
            }).ToList();

            return new ApiOkResponse<IEnumerable<TagCloudItemDto>>(tagCloud);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetTagCloudHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving tag cloud: {ex.Message}");
        }
    }
}