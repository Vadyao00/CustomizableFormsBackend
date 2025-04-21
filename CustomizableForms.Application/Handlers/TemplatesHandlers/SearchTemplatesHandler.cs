using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class SearchTemplatesHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<SearchTemplatesQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(SearchTemplatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var templates = await repository.Template.SearchTemplatesAsync(request.TemplateParameters, request.SearchTerm, trackChanges: false);
            var templatesDto = mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<(IEnumerable<TemplateDto>, MetaData)>((templatesDto, templates.MetaData));
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(SearchTemplatesHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error searching templates: {ex.Message}");
        }
    }
}