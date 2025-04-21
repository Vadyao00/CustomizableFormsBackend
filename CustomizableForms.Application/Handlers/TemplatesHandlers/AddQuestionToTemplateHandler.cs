using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Commands.TemplateCommands;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class AddQuestionToTemplateHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<AddQuestionToTemplateCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(AddQuestionToTemplateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: true);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (template.CreatorId != request.CurrentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to add questions to this template");
            }

            var existingQuestions = await repository.Question.GetTemplateQuestionsAsync(request.TemplateId, trackChanges: false);
            var questionsOfType = existingQuestions.Count(q => q.Type == request.QuestionDto.Type);
            if (questionsOfType >= 4)
            {
                return new ApiBadRequestResponse($"Maximum of 4 questions of type {request.QuestionDto.Type} reached");
            }

            var question = new Question
            {
                Id = Guid.NewGuid(),
                Title = request.QuestionDto.Title,
                Description = request.QuestionDto.Description,
                OrderIndex = request.QuestionDto.OrderIndex,
                ShowInResults = request.QuestionDto.ShowInResults,
                Type = request.QuestionDto.Type,
                TemplateId = request.TemplateId
            };

            repository.Question.CreateQuestion(question);
            await repository.SaveAsync();

            var questionResultDto = mapper.Map<QuestionDto>(question);
            return new ApiOkResponse<QuestionDto>(questionResultDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(AddQuestionToTemplateHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error adding question to template: {ex.Message}");
        }
    }
}