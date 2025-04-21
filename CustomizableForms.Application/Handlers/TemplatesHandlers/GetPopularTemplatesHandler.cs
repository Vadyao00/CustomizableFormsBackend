using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class GetPopularTemplatesHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetPopularTemplatesQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetPopularTemplatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var templates = await repository.Template.GetPopularTemplatesAsync(request.Count, trackChanges: false);
            var templatesDto = mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<IEnumerable<TemplateDto>>(templatesDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetPopularTemplatesHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving popular templates: {ex.Message}");
        }
    }
}