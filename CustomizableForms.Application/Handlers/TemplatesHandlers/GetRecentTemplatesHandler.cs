using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class GetRecentTemplatesHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetRecentTemplatesQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetRecentTemplatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var templates = await repository.Template.GetRecentTemplatesAsync(request.Count, trackChanges: false);
            var templatesDto = mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<IEnumerable<TemplateDto>>(templatesDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetRecentTemplatesHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving recent templates: {ex.Message}");
        }
    }
}