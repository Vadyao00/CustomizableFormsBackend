using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class GetPublicTemplatesHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetPublicTemplatesQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetPublicTemplatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var templates = await repository.Template.GetPublicTemplatesAsync(request.TemplateParameters, trackChanges: false);
            var templatesDto = mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<(IEnumerable<TemplateDto>, MetaData)>((templatesDto, templates.MetaData));
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetPublicTemplatesHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving public templates: {ex.Message}");
        }
    }
}