using Contracts.IRepositories;
using CustomizableForms.Application.Commands.TemplateCommands;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class DeleteQuestionHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<DeleteQuestionCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
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
                return new ApiBadRequestResponse("You do not have permission to delete questions from this template");
            }

            var question = await repository.Question.GetQuestionByIdAsync(request.QuestionId, trackChanges: true);
            if (question == null || question.TemplateId != request.TemplateId)
            {
                return new ApiBadRequestResponse("Question not found in this template");
            }

            repository.Question.DeleteQuestion(question);
            await repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(DeleteQuestionHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error deleting question: {ex.Message}");
        }
    }
}