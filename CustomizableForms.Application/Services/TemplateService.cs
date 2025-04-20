using AutoMapper;
using Contracts.IRepositories;
using Contracts.IServices;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;

namespace CustomizableForms.Application.Services;

public class TemplateService : ITemplateService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public TemplateService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiBaseResponse> GetPublicTemplatesAsync()
    {
        try
        {
            var templates = await _repository.Template.GetPublicTemplatesAsync(trackChanges: false);
            var templatesDto = _mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<IEnumerable<TemplateDto>>(templatesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetPublicTemplatesAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving public templates: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetAllowedTemplatesAsync(User currentUser)
    {
        try
        {
            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");
            
            var templates = await _repository.Template.GetAllowedTemplatesAsync(currentUser, isAdmin, trackChanges: false);
            var templatesDto = _mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<IEnumerable<TemplateDto>>(templatesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetAllowedTemplatesAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving allowed templates: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetTemplateByIdAsync(Guid templateId, User currentUser)
    {
        try
        {
            var template = await _repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (!template.IsPublic && currentUser != null && 
                template.CreatorId != currentUser.Id && 
                !template.AllowedUsers.Any(au => au.UserId == currentUser.Id) &&
                !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to view this template");
            }

            var templateDto = _mapper.Map<TemplateDto>(template);

            return new ApiOkResponse<TemplateDto>(templateDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetTemplateByIdAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetTemplateByIdWithoutTokenAsync(Guid templateId)
    {
        try
        {
            var template = await _repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            if (!template.IsPublic)
            {
                return new ApiBadRequestResponse("You do not have permission to view this template");
            }

            var templateDto = _mapper.Map<TemplateDto>(template);

            return new ApiOkResponse<TemplateDto>(templateDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetTemplateByIdAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template: {ex.Message}");
        }
    }
    
    public async Task<ApiBaseResponse> GetUserTemplatesAsync(TemplateParameters templateParameters, Guid userId, User currentUser)
    {
        try
        {
            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (currentUser.Id != userId && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to view these templates");
            }

            var templates = await _repository.Template.GetUserTemplatesAsync(templateParameters, userId, trackChanges: false);
            var templatesDto = _mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<(IEnumerable<TemplateDto>, MetaData)>((templatesDto, templates.MetaData));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetUserTemplatesAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving user templates: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetPopularTemplatesAsync(int count)
    {
        try
        {
            var templates = await _repository.Template.GetPopularTemplatesAsync(count, trackChanges: false);
            var templatesDto = _mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<IEnumerable<TemplateDto>>(templatesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetPopularTemplatesAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving popular templates: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetRecentTemplatesAsync(int count)
    {
        try
        {
            var templates = await _repository.Template.GetRecentTemplatesAsync(count, trackChanges: false);
            var templatesDto = _mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<IEnumerable<TemplateDto>>(templatesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetRecentTemplatesAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving recent templates: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> SearchTemplatesAsync(string searchTerm)
    {
        try
        {
            var templates = await _repository.Template.SearchTemplatesAsync(searchTerm, trackChanges: false);
            var templatesDto = _mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<IEnumerable<TemplateDto>>(templatesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(SearchTemplatesAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error searching templates: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> CreateTemplateAsync(TemplateForCreationDto templateDto, User currentUser)
    {
        try
        {
            if (currentUser == null)
            {
                return new ApiBadRequestResponse("User not found");
            }

            var template = new Template
            {
                Id = Guid.NewGuid(),
                Title = templateDto.Title,
                Description = templateDto.Description,
                Topic = templateDto.Topic,
                ImageUrl = templateDto.ImageUrl,
                IsPublic = templateDto.IsPublic,
                CreatedAt = DateTime.UtcNow,
                CreatorId = currentUser.Id
            };

            _repository.Template.CreateTemplate(template);
            
            if (templateDto.Tags != null && templateDto.Tags.Any())
            {
                foreach (var tagName in templateDto.Tags)
                {
                    var tag = await _repository.Tag.GetTagByNameAsync(tagName, trackChanges: true);
                    if (tag == null)
                    {
                        tag = new Tag
                        {
                            Id = Guid.NewGuid(),
                            Name = tagName
                        };
                        _repository.Tag.CreateTag(tag);
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

            if (!template.IsPublic && templateDto.AllowedUserEmails != null && templateDto.AllowedUserEmails.Any())
            {
                foreach (var email in templateDto.AllowedUserEmails)
                {
                    var user = await _repository.User.GetUserByEmailAsync(email);
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

            await _repository.SaveAsync();

            var savedTemplate = await _repository.Template.GetTemplateByIdAsync(template.Id, trackChanges: false);
            var templateResultDto = _mapper.Map<TemplateDto>(savedTemplate);

            return new ApiOkResponse<TemplateDto>(templateResultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(CreateTemplateAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error creating template: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> UpdateTemplateAsync(Guid templateId, TemplateForUpdateDto templateDto, User currentUser)
    {
        try
        {
            var template = await _repository.Template.GetTemplateByIdAsync(templateId, trackChanges: true);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (template.CreatorId != currentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to update this template");
            }

            template.Title = templateDto.Title;
            template.Description = templateDto.Description;
            template.Topic = templateDto.Topic;
            template.ImageUrl = templateDto.ImageUrl;
            template.IsPublic = templateDto.IsPublic;
            template.UpdatedAt = DateTime.UtcNow;

            if (template.TemplateTags != null)
            {
                foreach (var tt in template.TemplateTags.ToList())
                {
                    template.TemplateTags.Remove(tt);
                }
            }

            if (templateDto.Tags != null && templateDto.Tags.Any())
            {
                template.TemplateTags ??= new List<TemplateTag>();
                foreach (var tagName in templateDto.Tags)
                {
                    var tag = await _repository.Tag.GetTagByNameAsync(tagName, trackChanges: true);
                    if (tag == null)
                    {
                        tag = new Tag
                        {
                            Id = Guid.NewGuid(),
                            Name = tagName
                        };
                        _repository.Tag.CreateTag(tag);
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

            if (!template.IsPublic && templateDto.AllowedUserEmails != null && templateDto.AllowedUserEmails.Any())
            {
                template.AllowedUsers ??= new List<TemplateAccess>();
                foreach (var email in templateDto.AllowedUserEmails)
                {
                    var user = await _repository.User.GetUserByEmailAsync(email);
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

            _repository.Template.UpdateTemplate(template);
            await _repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(UpdateTemplateAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error updating template: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> DeleteTemplateAsync(Guid templateId, User currentUser)
    {
        try
        {
            var template = await _repository.Template.GetTemplateByIdAsync(templateId, trackChanges: true);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (template.CreatorId != currentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to delete this template");
            }

            _repository.Template.DeleteTemplate(template);
            await _repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(DeleteTemplateAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error deleting template: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetTemplateQuestionsAsync(Guid templateId, User currentUser)
    {
        try
        {
            var template = await _repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            if (!template.IsPublic && currentUser == null)
            {
                return new ApiBadRequestResponse("You do not have permission to view this template's questions");
            }

            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (!template.IsPublic && currentUser != null && 
                template.CreatorId != currentUser.Id && 
                !template.AllowedUsers.Any(au => au.UserId == currentUser.Id) &&
                !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to view this template's questions");
            }

            var questions = await _repository.Question.GetTemplateQuestionsAsync(templateId, trackChanges: false);
            var questionsDto = _mapper.Map<IEnumerable<QuestionDto>>(questions);

            return new ApiOkResponse<IEnumerable<QuestionDto>>(questionsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetTemplateQuestionsAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template questions: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetTemplateQuestionsWithoutUserAsync(Guid templateId)
    {
        try
        {
            var template = await _repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            if (!template.IsPublic)
            {
                return new ApiBadRequestResponse("You do not have permission to view this template's questions");
            }

            var questions = await _repository.Question.GetTemplateQuestionsAsync(templateId, trackChanges: false);
            var questionsDto = _mapper.Map<IEnumerable<QuestionDto>>(questions);

            return new ApiOkResponse<IEnumerable<QuestionDto>>(questionsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetTemplateQuestionsAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template questions: {ex.Message}");
        }
    }
    
    public async Task<ApiBaseResponse> AddQuestionToTemplateAsync(Guid templateId, QuestionForCreationDto questionDto, User currentUser)
    {
        try
        {
            var template = await _repository.Template.GetTemplateByIdAsync(templateId, trackChanges: true);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (template.CreatorId != currentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to add questions to this template");
            }

            var existingQuestions = await _repository.Question.GetTemplateQuestionsAsync(templateId, trackChanges: false);
            var questionsOfType = existingQuestions.Count(q => q.Type == questionDto.Type);
            if (questionsOfType >= 4)
            {
                return new ApiBadRequestResponse($"Maximum of 4 questions of type {questionDto.Type} reached");
            }

            var question = new Question
            {
                Id = Guid.NewGuid(),
                Title = questionDto.Title,
                Description = questionDto.Description,
                OrderIndex = questionDto.OrderIndex,
                ShowInResults = questionDto.ShowInResults,
                Type = questionDto.Type,
                TemplateId = templateId
            };

            _repository.Question.CreateQuestion(question);
            await _repository.SaveAsync();

            var questionResultDto = _mapper.Map<QuestionDto>(question);
            return new ApiOkResponse<QuestionDto>(questionResultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(AddQuestionToTemplateAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error adding question to template: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> UpdateQuestionAsync(Guid templateId, Guid questionId, QuestionForUpdateDto questionDto, User currentUser)
    {
        try
        {
            var template = await _repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (template.CreatorId != currentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to update questions in this template");
            }

            var question = await _repository.Question.GetQuestionByIdAsync(questionId, trackChanges: true);
            if (question == null || question.TemplateId != templateId)
            {
                return new ApiBadRequestResponse("Question not found in this template");
            }

            question.Title = questionDto.Title;
            question.Description = questionDto.Description;
            question.ShowInResults = questionDto.ShowInResults;

            _repository.Question.UpdateQuestion(question);
            await _repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(UpdateQuestionAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error updating question: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> DeleteQuestionAsync(Guid templateId, Guid questionId, User currentUser)
    {
        try
        {
            var template = await _repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (template.CreatorId != currentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to delete questions from this template");
            }

            var question = await _repository.Question.GetQuestionByIdAsync(questionId, trackChanges: true);
            if (question == null || question.TemplateId != templateId)
            {
                return new ApiBadRequestResponse("Question not found in this template");
            }

            _repository.Question.DeleteQuestion(question);
            await _repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(DeleteQuestionAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error deleting question: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> ReorderQuestionsAsync(Guid templateId, List<Guid> questionIds, User currentUser)
    {
        try
        {
            var template = await _repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (template.CreatorId != currentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to reorder questions in this template");
            }

            var questions = await _repository.Question.GetTemplateQuestionsAsync(templateId, trackChanges: true);
            var questionDict = questions.ToDictionary(q => q.Id, q => q);

            for (int i = 0; i < questionIds.Count; i++)
            {
                var questionId = questionIds[i];
                if (questionDict.TryGetValue(questionId, out var question))
                {
                    question.OrderIndex = i;
                    _repository.Question.UpdateQuestion(question);
                }
            }

            await _repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(ReorderQuestionsAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error reordering questions: {ex.Message}");
        }
    }
}