using Contracts.IRepositories;
using CustomizableForms.Application.Commands.TemplateCommands;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class ReorderQuestionsHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<ReorderQuestionsCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(ReorderQuestionsCommand request, CancellationToken cancellationToken)
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
                return new ApiBadRequestResponse("You do not have permission to reorder questions in this template");
            }

            var questions = await repository.Question.GetTemplateQuestionsAsync(request.TemplateId, trackChanges: true);
            var questionDict = questions.ToDictionary(q => q.Id, q => q);

            for (int i = 0; i < request.QuestionIds.Count; i++)
            {
                var questionId = request.QuestionIds[i];
                if (questionDict.TryGetValue(questionId, out var question))
                {
                    question.OrderIndex = i;
                    repository.Question.UpdateQuestion(question);
                }
            }

            await repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(ReorderQuestionsHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error reordering questions: {ex.Message}");
        }
    }
}