using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class GetTemplateQuestionsHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetTemplateQuestionsQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetTemplateQuestionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            if (!template.IsPublic && request.CurrentUser == null)
            {
                return new ApiBadRequestResponse("You do not have permission to view this template's questions");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (!template.IsPublic && request.CurrentUser != null && 
                template.CreatorId != request.CurrentUser.Id && 
                !template.AllowedUsers.Any(au => au.UserId == request.CurrentUser.Id) &&
                !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to view this template's questions");
            }

            var questions = await repository.Question.GetTemplateQuestionsAsync(request.TemplateId, trackChanges: false);
            var questionsDto = mapper.Map<IEnumerable<QuestionDto>>(questions);

            return new ApiOkResponse<IEnumerable<QuestionDto>>(questionsDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetTemplateQuestionsHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template questions: {ex.Message}");
        }
    }
}