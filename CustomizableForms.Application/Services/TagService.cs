using AutoMapper;
using Contracts.IRepositories;
using Contracts.IServices;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;

namespace CustomizableForms.Application.Services;

public class TagService : ITagService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public TagService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiBaseResponse> GetAllTagsAsync()
    {
        try
        {
            var tags = await _repository.Tag.GetAllTagsAsync(trackChanges: false);
            var tagsDto = _mapper.Map<IEnumerable<TagDto>>(tags);

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
            _logger.LogError($"Error in {nameof(GetAllTagsAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving tags: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> SearchTagsAsync(string searchTerm)
    {
        try
        {
            var tags = await _repository.Tag.SearchTagsAsync(searchTerm, trackChanges: false);
            var tagsDto = _mapper.Map<IEnumerable<TagDto>>(tags);

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
            _logger.LogError($"Error in {nameof(SearchTagsAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error searching tags: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetTagCloudAsync()
    {
        try
        {
            var tags = await _repository.Tag.GetAllTagsAsync(trackChanges: false);
            
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
            _logger.LogError($"Error in {nameof(GetTagCloudAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving tag cloud: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetTemplatesByTagAsync(string tagName)
    {
        try
        {
            var tag = await _repository.Tag.GetTagByNameAsync(tagName, trackChanges: false);
            if (tag == null)
            {
                return new ApiBadRequestResponse("Tag not found");
            }

            var templates = await _repository.Template.GetPublicTemplatesAsync(trackChanges: false);
            var templatesWithTag = templates
                .Where(t => t.TemplateTags != null && t.TemplateTags.Any(tt => tt.TagId == tag.Id))
                .ToList();

            var templatesDto = _mapper.Map<IEnumerable<TemplateDto>>(templatesWithTag);

            foreach (var templateDto in templatesDto)
            {
                var template = templatesWithTag.FirstOrDefault(t => t.Id == templateDto.Id);
                if (template != null)
                {
                    templateDto.LikesCount = template.Likes?.Count ?? 0;
                    templateDto.CommentsCount = template.Comments?.Count ?? 0;
                    templateDto.FormsCount = template.Forms?.Count ?? 0;
                    
                    if (template.TemplateTags != null)
                    {
                        templateDto.Tags = template.TemplateTags
                            .Select(tt => tt.Tag?.Name)
                            .Where(name => !string.IsNullOrEmpty(name))
                            .ToList();
                    }
                }
            }

            return new ApiOkResponse<IEnumerable<TemplateDto>>(templatesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetTemplatesByTagAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving templates by tag: {ex.Message}");
        }
    }
}