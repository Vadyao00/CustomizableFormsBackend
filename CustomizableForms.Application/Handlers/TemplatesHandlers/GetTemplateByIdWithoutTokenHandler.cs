using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class GetTemplateByIdWithoutTokenHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetTemplateByIdWithoutTokenQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetTemplateByIdWithoutTokenQuery request, CancellationToken cancellationToken)
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
                return new ApiBadRequestResponse("You do not have permission to view this template");
            }

            var templateDto = mapper.Map<TemplateDto>(template);

            return new ApiOkResponse<TemplateDto>(templateDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetTemplateByIdWithoutTokenHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template: {ex.Message}");
        }
    }
}