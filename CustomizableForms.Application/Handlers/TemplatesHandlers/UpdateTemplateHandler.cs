using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.TemplateCommands;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class UpdateTemplateHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<UpdateTemplateCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: true);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (template.CreatorId != request.CurrentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to update this template");
            }

            template.Title = request.TemplateDto.Title;
            template.Description = request.TemplateDto.Description;
            template.Topic = request.TemplateDto.Topic;
            template.ImageUrl = request.TemplateDto.ImageUrl;
            template.IsPublic = request.TemplateDto.IsPublic;
            template.UpdatedAt = DateTime.UtcNow;

            if (template.TemplateTags != null)
            {
                foreach (var tt in template.TemplateTags.ToList())
                {
                    template.TemplateTags.Remove(tt);
                }
            }

            if (request.TemplateDto.Tags != null && request.TemplateDto.Tags.Any())
            {
                template.TemplateTags ??= new List<TemplateTag>();
                foreach (var tagName in request.TemplateDto.Tags)
                {
                    var tag = await repository.Tag.GetTagByNameAsync(tagName, trackChanges: true);
                    if (tag == null)
                    {
                        tag = new Tag
                        {
                            Id = Guid.NewGuid(),
                            Name = tagName
                        };
                        repository.Tag.CreateTag(tag);
                    }

                    var templateTag = new TemplateTag
                    {
                        TemplateId = template.Id,
                        TagId = tag.Id
                    };
                    template.TemplateTags.Add(templateTag);
                }
            }

            if (template.AllowedUsers != null)
            {
                foreach (var au in template.AllowedUsers.ToList())
                {
                    template.AllowedUsers.Remove(au);
                }
            }

            if (!template.IsPublic && request.TemplateDto.AllowedUserEmails != null && request.TemplateDto.AllowedUserEmails.Any())
            {
                template.AllowedUsers ??= new List<TemplateAccess>();
                foreach (var email in request.TemplateDto.AllowedUserEmails)
                {
                    var user = await repository.User.GetUserByEmailAsync(email);
                    if (user != null)
                    {
                        var templateAccess = new TemplateAccess
                        {
                            TemplateId = template.Id,
                            UserId = user.Id
                        };
                        template.AllowedUsers.Add(templateAccess);
                    }
                }
            }

            repository.Template.UpdateTemplate(template);
            await repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(UpdateTemplateHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error updating template: {ex.Message}");
        }
    }
}