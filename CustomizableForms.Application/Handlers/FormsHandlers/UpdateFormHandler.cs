using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.FormsCommands;
using CustomizableForms.Domain.Enums;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.FormsHandlers;

public class UpdateFormHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<UpdateFormCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(UpdateFormCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var form = await repository.Form.GetFormByIdAsync(request.FormId, trackChanges: true);
            if (form == null)
            {
                return new ApiBadRequestResponse("Form not found");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (form.UserId != request.CurrentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to update this form");
            }

            var questions = await repository.Question.GetTemplateQuestionsAsync(form.TemplateId, trackChanges: false);
            var questionMap = questions.ToDictionary(q => q.Id);

            var existingAnswers = await repository.Answer.GetFormAnswersAsync(request.FormId, trackChanges: true);
            var existingAnswerMap = existingAnswers.ToDictionary(a => a.Id);

            foreach (var answerUpdate in request.FormDto.Answers)
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

            foreach (var answerUpdate in request.FormDto.Answers)
            {
                if (existingAnswerMap.TryGetValue(answerUpdate.Id, out var existingAnswer))
                {
                    existingAnswer.StringValue = answerUpdate.StringValue;
                    existingAnswer.IntegerValue = answerUpdate.IntegerValue;
                    existingAnswer.BooleanValue = answerUpdate.BooleanValue;
                    repository.Answer.UpdateAnswer(existingAnswer);
                }
            }

            await repository.SaveAsync();
            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(UpdateFormHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error updating form: {ex.Message}");
        }
    }
}