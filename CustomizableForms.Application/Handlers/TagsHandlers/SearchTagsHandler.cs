using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TagsQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TagsHandlers;

public class SearchTagsHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<SearchTagsQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(SearchTagsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var tags = await repository.Tag.SearchTagsAsync(request.SearchTerm, trackChanges: false);
            var tagsDto = mapper.Map<IEnumerable<TagDto>>(tags);

            foreach (var tagDto in tagsDto)
            {
                var tag = tags.FirstOrDefault(t => t.Id == tagDto.Id);
                if (tag != null)
                {
                    tagDto.TemplatesCount = tag.TemplateTags?.Count ?? 0;
                }
            }

            return new ApiOkResponse<IEnumerable<TagDto>>(tagsDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(SearchTagsHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error searching tags: {ex.Message}");
        }
    }
}