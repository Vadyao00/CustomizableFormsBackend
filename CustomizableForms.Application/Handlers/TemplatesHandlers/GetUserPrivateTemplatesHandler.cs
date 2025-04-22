using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class GetUserPrivateTemplatesHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetUserPrivateTemplatesQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetUserPrivateTemplatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (request.CurrentUser.Id != request.UserId && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to view these templates");
            }

            var templates = await repository.Template.GetUserPrivateTemplatesAsync(request.TemplateParameters, request.UserId, trackChanges: false);
            var templatesDto = mapper.Map<IEnumerable<TemplateDto>>(templates);

            return new ApiOkResponse<(IEnumerable<TemplateDto>, MetaData)>((templatesDto, templates.MetaData));
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetUserPrivateTemplatesHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving user templates: {ex.Message}");
        }
    }
}