using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.FormsCommands;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Enums;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.FormsHandlers;

public class SubmitFormHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<SubmitFormCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(SubmitFormCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (!template.IsPublic && 
                template.CreatorId != request.CurrentUser.Id && 
                !template.AllowedUsers.Any(au => au.UserId == request.CurrentUser.Id) &&
                !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to submit this form");
            }

            var questions = await repository.Question.GetTemplateQuestionsAsync(request.TemplateId, trackChanges: false);
            var questionMap = questions.ToDictionary(q => q.Id);

            foreach (var answer in request.FormDto.Answers)
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
                TemplateId = request.TemplateId,
                UserId = request.CurrentUser.Id,
                SubmittedAt = DateTime.UtcNow,
                Answers = new List<Answer>()
            };

            foreach (var answerDto in request.FormDto.Answers)
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

            repository.Form.CreateForm(form);
            await repository.SaveAsync();

            var createdForm = await repository.Form.GetFormByIdAsync(form.Id, trackChanges: false);
            var resultFormDto = mapper.Map<FormDto>(createdForm);

            return new ApiOkResponse<FormDto>(resultFormDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(SubmitFormHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error submitting form: {ex.Message}");
        }
    }
}