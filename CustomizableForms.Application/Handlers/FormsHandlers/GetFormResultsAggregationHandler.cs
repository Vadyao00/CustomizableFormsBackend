using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.FormsQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Enums;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.FormsHandlers;

public class GetFormResultsAggregationHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<GetFormResultsAggregationQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetFormResultsAggregationQuery request, CancellationToken cancellationToken)
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
                return new ApiBadRequestResponse("You do not have permission to view these results");
            }

            var forms = await repository.Form.GetAllTemplateFormsAsync(request.TemplateId, trackChanges: false);
            var questions = await repository.Question.GetTemplateQuestionsAsync(request.TemplateId, trackChanges: false);

            var aggregation = new FormResultsAggregationDto
            {
                TemplateId = request.TemplateId,
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
            logger.LogError($"Error in {nameof(GetFormResultsAggregationHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving form results aggregation: {ex.Message}");
        }
    }
}