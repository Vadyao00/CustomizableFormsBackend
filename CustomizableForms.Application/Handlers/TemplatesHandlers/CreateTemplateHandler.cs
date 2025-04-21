using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.TemplateCommands;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class CreateTemplateHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<CreateTemplateCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.CurrentUser == null)
            {
                return new ApiBadRequestResponse("User not found");
            }

            var template = new Template
            {
                Id = Guid.NewGuid(),
                Title = request.TemplateDto.Title,
                Description = request.TemplateDto.Description,
                Topic = request.TemplateDto.Topic,
                ImageUrl = request.TemplateDto.ImageUrl,
                IsPublic = request.TemplateDto.IsPublic,
                CreatedAt = DateTime.UtcNow,
                CreatorId = request.CurrentUser.Id
            };

            repository.Template.CreateTemplate(template);
            
            if (request.TemplateDto.Tags != null && request.TemplateDto.Tags.Any())
            {
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
                    template.TemplateTags ??= new List<TemplateTag>();
                    template.TemplateTags.Add(templateTag);
                }
            }

            if (!template.IsPublic && request.TemplateDto.AllowedUserEmails != null && request.TemplateDto.AllowedUserEmails.Any())
            {
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
                        template.AllowedUsers ??= new List<TemplateAccess>();
                        template.AllowedUsers.Add(templateAccess);
                    }
                }
            }

            await repository.SaveAsync();

            var savedTemplate = await repository.Template.GetTemplateByIdAsync(template.Id, trackChanges: false);
            var templateResultDto = mapper.Map<TemplateDto>(savedTemplate);

            return new ApiOkResponse<TemplateDto>(templateResultDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(CreateTemplateHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error creating template: {ex.Message}");
        }
    }
}