using AutoMapper;
using Contracts.IRepositories;
using Contracts.IServices;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Enums;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;

namespace CustomizableForms.Application.Services;

public class FormService : IFormService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public FormService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiBaseResponse> GetUserFormsAsync(FormParameters formParameters, User currentUser)
    {
        try
        {
            var forms = await _repository.Form.GetUserFormsAsync(formParameters, currentUser.Id, trackChanges: false);
            var formsDto = _mapper.Map<IEnumerable<FormDto>>(forms);

            return new ApiOkResponse<(IEnumerable<FormDto>, MetaData)>((formsDto, forms.MetaData));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetUserFormsAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving user forms: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetTemplateFormsAsync(FormParameters formParameters, Guid templateId, User currentUser)
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
                return new ApiBadRequestResponse("You do not have permission to view these forms");
            }

            var forms = await _repository.Form.GetTemplateFormsAsync(formParameters, templateId, trackChanges: false);
            var formsDto = _mapper.Map<IEnumerable<FormDto>>(forms);

            return new ApiOkResponse<(IEnumerable<FormDto>, MetaData)>((formsDto, forms.MetaData));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetTemplateFormsAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template forms: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetFormByIdAsync(Guid formId, User currentUser)
    {
        try
        {
            var form = await _repository.Form.GetFormByIdAsync(formId, trackChanges: false);
            if (form == null)
            {
                return new ApiBadRequestResponse("Form not found");
            }

            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (form.UserId != currentUser.Id && form.Template.CreatorId != currentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to view this form");
            }

            var formDto = _mapper.Map<FormDto>(form);
            return new ApiOkResponse<FormDto>(formDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetFormByIdAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving form: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> SubmitFormAsync(Guid templateId, FormForSubmissionDto formDto, User currentUser)
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

            if (!template.IsPublic && 
                template.CreatorId != currentUser.Id && 
                !template.AllowedUsers.Any(au => au.UserId == currentUser.Id) &&
                !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to submit this form");
            }

            var questions = await _repository.Question.GetTemplateQuestionsAsync(templateId, trackChanges: false);
            var questionMap = questions.ToDictionary(q => q.Id);

            foreach (var answer in formDto.Answers)
            {
                if (!questionMap.TryGetValue(answer.QuestionId, out var question))
                {
                    return new ApiBadRequestResponse($"Question with ID {answer.QuestionId} does not exist in this template");
                }

                switch (question.Type)
                {
                    case QuestionType.SingleLineString:
                    case QuestionType.MultiLineText:
                        if (string.IsNullOrEmpty(answer.StringValue))
                        {
                            return new ApiBadRequestResponse($"Text answer required for question: {question.Title}");
                        }
                        break;
                    case QuestionType.Integer:
                        if (!answer.IntegerValue.HasValue)
                        {
                            return new ApiBadRequestResponse($"Integer answer required for question: {question.Title}");
                        }
                        break;
                    case QuestionType.Checkbox:
                        if (!answer.BooleanValue.HasValue)
                        {
                            return new ApiBadRequestResponse($"Boolean answer required for question: {question.Title}");
                        }
                        break;
                }
            }

            var form = new Form
            {
                Id = Guid.NewGuid(),
                TemplateId = templateId,
                UserId = currentUser.Id,
                SubmittedAt = DateTime.UtcNow,
                Answers = new List<Answer>()
            };

            foreach (var answerDto in formDto.Answers)
            {
                var answer = new Answer
                {
                    Id = Guid.NewGuid(),
                    FormId = form.Id,
                    QuestionId = answerDto.QuestionId,
                    StringValue = answerDto.StringValue,
                    IntegerValue = answerDto.IntegerValue,
                    BooleanValue = answerDto.BooleanValue
                };
                form.Answers.Add(answer);
            }

            _repository.Form.CreateForm(form);
            await _repository.SaveAsync();

            var createdForm = await _repository.Form.GetFormByIdAsync(form.Id, trackChanges: false);
            var resultFormDto = _mapper.Map<FormDto>(createdForm);

            return new ApiOkResponse<FormDto>(resultFormDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(SubmitFormAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error submitting form: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> UpdateFormAsync(Guid formId, FormForUpdateDto formDto, User currentUser)
    {
        try
        {
            var form = await _repository.Form.GetFormByIdAsync(formId, trackChanges: true);
            if (form == null)
            {
                return new ApiBadRequestResponse("Form not found");
            }

            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (form.UserId != currentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to update this form");
            }

            var questions = await _repository.Question.GetTemplateQuestionsAsync(form.TemplateId, trackChanges: false);
            var questionMap = questions.ToDictionary(q => q.Id);

            var existingAnswers = await _repository.Answer.GetFormAnswersAsync(formId, trackChanges: true);
            var existingAnswerMap = existingAnswers.ToDictionary(a => a.Id);

            foreach (var answerUpdate in formDto.Answers)
            {
                if (!existingAnswerMap.TryGetValue(answerUpdate.Id, out var existingAnswer))
                {
                    return new ApiBadRequestResponse($"Answer with ID {answerUpdate.Id} does not exist in this form");
                }

                if (!questionMap.TryGetValue(existingAnswer.QuestionId, out var question))
                {
                    return new ApiBadRequestResponse($"Question for answer ID {answerUpdate.Id} does not exist");
                }

                switch (question.Type)
                {
                    case QuestionType.SingleLineString:
                    case QuestionType.MultiLineText:
                        if (string.IsNullOrEmpty(answerUpdate.StringValue))
                        {
                            return new ApiBadRequestResponse($"Text answer required for question: {question.Title}");
                        }
                        break;
                    case QuestionType.Integer:
                        if (!answerUpdate.IntegerValue.HasValue)
                        {
                            return new ApiBadRequestResponse($"Integer answer required for question: {question.Title}");
                        }
                        break;
                    case QuestionType.Checkbox:
                        if (!answerUpdate.BooleanValue.HasValue)
                        {
                            return new ApiBadRequestResponse($"Boolean answer required for question: {question.Title}");
                        }
                        break;
                }
            }

            foreach (var answerUpdate in formDto.Answers)
            {
                if (existingAnswerMap.TryGetValue(answerUpdate.Id, out var existingAnswer))
                {
                    existingAnswer.StringValue = answerUpdate.StringValue;
                    existingAnswer.IntegerValue = answerUpdate.IntegerValue;
                    existingAnswer.BooleanValue = answerUpdate.BooleanValue;
                    _repository.Answer.UpdateAnswer(existingAnswer);
                }
            }

            await _repository.SaveAsync();
            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(UpdateFormAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error updating form: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> DeleteFormAsync(Guid formId, User currentUser)
    {
        try
        {
            var form = await _repository.Form.GetFormByIdAsync(formId, trackChanges: true);
            if (form == null)
            {
                return new ApiBadRequestResponse("Form not found");
            }

            bool isAdmin = false;
            var userRoles = await _repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (form.UserId != currentUser.Id && form.Template.CreatorId != currentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to delete this form");
            }

            _repository.Form.DeleteForm(form);
            await _repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(DeleteFormAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error deleting form: {ex.Message}");
        }
    }

    public async Task<ApiBaseResponse> GetFormResultsAggregationAsync(Guid templateId, User currentUser)
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
                return new ApiBadRequestResponse("You do not have permission to view these results");
            }

            var forms = await _repository.Form.GetAllTemplateFormsAsync(templateId, trackChanges: false);
            var questions = await _repository.Question.GetTemplateQuestionsAsync(templateId, trackChanges: false);

            var aggregation = new FormResultsAggregationDto
            {
                TemplateId = templateId,
                TemplateTitle = template.Title,
                TotalResponses = forms.Count(),
                QuestionResults = new List<QuestionResultDto>()
            };

            foreach (var question in questions.Where(q => q.ShowInResults))
            {
                var questionResult = new QuestionResultDto
                {
                    QuestionId = question.Id,
                    QuestionTitle = question.Title,
                    Type = question.Type
                };

                var questionAnswers = forms
                    .SelectMany(f => f.Answers)
                    .Where(a => a.QuestionId == question.Id)
                    .ToList();

                // Process based on question type
                switch (question.Type)
                {
                    case QuestionType.Integer:
                        var integerValues = questionAnswers
                            .Where(a => a.IntegerValue.HasValue)
                            .Select(a => a.IntegerValue.Value)
                            .ToList();

                        if (integerValues.Any())
                        {
                            questionResult.AverageValue = integerValues.Average();
                            questionResult.MinValue = integerValues.Min();
                            questionResult.MaxValue = integerValues.Max();
                        }
                        break;

                    case QuestionType.SingleLineString:
                    case QuestionType.MultiLineText:
                        var stringValues = questionAnswers
                            .Where(a => !string.IsNullOrEmpty(a.StringValue))
                            .Select(a => a.StringValue)
                            .ToList();

                        if (stringValues.Any())
                        {
                            // Group by value and count occurrences
                            var valueGroups = stringValues
                                .GroupBy(v => v)
                                .Select(g => new StringValueCountDto { Value = g.Key, Count = g.Count() })
                                .OrderByDescending(v => v.Count)
                                .Take(5)
                                .ToList();

                            questionResult.MostCommonValues = valueGroups;
                        }
                        break;

                    case QuestionType.Checkbox:
                        var booleanValues = questionAnswers
                            .Where(a => a.BooleanValue.HasValue)
                            .Select(a => a.BooleanValue.Value)
                            .ToList();

                        if (booleanValues.Any())
                        {
                            var trueCount = booleanValues.Count(v => v);
                            var falseCount = booleanValues.Count(v => !v);

                            questionResult.TrueCount = trueCount;
                            questionResult.FalseCount = falseCount;
                            questionResult.TruePercentage = (double)trueCount / booleanValues.Count * 100;
                        }
                        break;
                }

                aggregation.QuestionResults.Add(questionResult);
            }

            return new ApiOkResponse<FormResultsAggregationDto>(aggregation);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetFormResultsAggregationAsync)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving form results aggregation: {ex.Message}");
        }
    }
}