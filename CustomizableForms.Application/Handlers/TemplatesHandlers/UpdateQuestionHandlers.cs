using Contracts.IRepositories;
using CustomizableForms.Application.Commands.TemplateCommands;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class UpdateQuestionHandlers(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<UpdateQuestionCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
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

            if (template.CreatorId != request.CurrentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to update questions in this template");
            }

            var question = await repository.Question.GetQuestionByIdAsync(request.QuestionId, trackChanges: true);
            if (question == null || question.TemplateId != request.TemplateId)
            {
                return new ApiBadRequestResponse("Question not found in this template");
            }

            question.Title = request.QuestionDto.Title;
            question.Description = request.QuestionDto.Description;
            question.ShowInResults = request.QuestionDto.ShowInResults;

            repository.Question.UpdateQuestion(question);
            await repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(UpdateQuestionHandlers)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error updating question: {ex.Message}");
        }
    }
}