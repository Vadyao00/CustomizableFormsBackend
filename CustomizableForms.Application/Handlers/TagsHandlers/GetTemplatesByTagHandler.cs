using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TagsQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TagsHandlers;

public class GetTemplatesByTagHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetTemplatesByTagQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetTemplatesByTagQuery request, CancellationToken cancellationToken)
{
    try
    {
        bool isAdmin = false;
        if (request.CurrentUser is not null)
        {
            var userRoles = await repository.Role.GetUserRolesAsync(
                request.CurrentUser.Id, 
                trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");
        }
        
        var templates = await repository.Template.GetTemplatesByTagAsync(
            request.TemplateParameters, 
            request.TagName, 
            request.CurrentUser, 
            isAdmin, 
            trackChanges: false);
        
        if (templates.Count == 0)
        {
            var tag = await repository.Tag.GetTagByNameAsync(
                request.TagName, 
                trackChanges: false);
                
            if (tag == null)
            {
                return new ApiBadRequestResponse("Тег не найден");
            }
        }
        
        var templatesDto = mapper.Map<IEnumerable<TemplateDto>>(templates);

        foreach (var templateDto in templatesDto)
        {
            var template = templates.FirstOrDefault(t => t.Id == templateDto.Id);
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

        return new ApiOkResponse<(IEnumerable<TemplateDto>, MetaData)>((templatesDto, templates.MetaData));
    }
    catch (Exception ex)
    {
        logger.LogError($"Error in {nameof(GetTemplatesByTagHandler)}: {ex.Message}");
        return new ApiBadRequestResponse($"Ошибка при получении шаблонов по тегу: {ex.Message}");
    }
}
}