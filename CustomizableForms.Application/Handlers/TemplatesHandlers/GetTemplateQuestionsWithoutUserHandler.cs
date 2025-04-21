using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class GetTemplateQuestionsWithoutUserHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetTemplateQuestionsWithoutUserQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetTemplateQuestionsWithoutUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            if (!template.IsPublic)
            {
                return new ApiBadRequestResponse("You do not have permission to view this template's questions");
            }

            var questions = await repository.Question.GetTemplateQuestionsAsync(request.TemplateId, trackChanges: false);
            var questionsDto = mapper.Map<IEnumerable<QuestionDto>>(questions);

            return new ApiOkResponse<IEnumerable<QuestionDto>>(questionsDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetTemplateQuestionsWithoutUserHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template questions: {ex.Message}");
        }
    }
}