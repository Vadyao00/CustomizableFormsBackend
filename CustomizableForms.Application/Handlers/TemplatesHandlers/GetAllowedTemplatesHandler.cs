using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class GetAllowedTemplatesHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetAllowedTemplatesQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetAllowedTemplatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");
            
            var templates = await repository.Template.GetAllowedTemplatesAsync(request.TemplateParameters, request.CurrentUser, isAdmin, trackChanges: false);
            var templatesDto = mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<(IEnumerable<TemplateDto>, MetaData)>((templatesDto, templates.MetaData));
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetAllowedTemplatesHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving allowed templates: {ex.Message}");
        }
    }
}