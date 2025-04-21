using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.TemplatesQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class GetTemplateByIdHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetTemplateByIdQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
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

            if (!template.IsPublic && request.CurrentUser != null && 
                template.CreatorId != request.CurrentUser.Id && 
                !template.AllowedUsers.Any(au => au.UserId == request.CurrentUser.Id) &&
                !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to view this template");
            }

            var templateDto = mapper.Map<TemplateDto>(template);

            return new ApiOkResponse<TemplateDto>(templateDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetTemplateByIdHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template: {ex.Message}");
        }
    }
}