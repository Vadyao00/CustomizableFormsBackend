using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TagsQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
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
            var tag = await repository.Tag.GetTagByNameAsync(request.TagName, trackChanges: false);
            if (tag == null)
            {
                return new ApiBadRequestResponse("Tag not found");
            }

            IEnumerable<Template> templates;
            List<Template> templatesWithTag;
            
            if (request.CurrentUser is not null)
            {
                bool isAdmin = false;
                var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
                isAdmin = userRoles.Any(r => r.Name == "Admin");

                templates = await repository.Template.GetAllowedTemplatesAsync(request.CurrentUser, isAdmin, trackChanges: false);
                templatesWithTag = templates
                    .Where(t => t.TemplateTags != null && t.TemplateTags.Any(tt => tt.TagId == tag.Id))
                    .ToList();    
            }
            else
            {
                templates = await repository.Template.GetPublicTemplatesAsync(trackChanges: false);
                templatesWithTag = templates
                    .Where(t => t.TemplateTags != null && t.TemplateTags.Any(tt => tt.TagId == tag.Id))
                    .ToList();   
            }
            

            var templatesDto = mapper.Map<IEnumerable<TemplateDto>>(templatesWithTag);

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
            logger.LogError($"Error in {nameof(GetTemplatesByTagHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving templates by tag: {ex.Message}");
        }
    }
}