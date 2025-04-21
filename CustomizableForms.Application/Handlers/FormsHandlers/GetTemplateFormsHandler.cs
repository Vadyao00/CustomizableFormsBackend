using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.FormsQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.FormsHandlers;

public class GetTemplateFormsHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetTemplateFormsQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetTemplateFormsQuery request, CancellationToken cancellationToken)
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
                return new ApiBadRequestResponse("You do not have permission to view these forms");
            }

            var forms = await repository.Form.GetTemplateFormsAsync(request.FormParameters, request.TemplateId, trackChanges: false);
            var formsDto = mapper.Map<IEnumerable<FormDto>>(forms);

            return new ApiOkResponse<(IEnumerable<FormDto>, MetaData)>((formsDto, forms.MetaData));
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetTemplateFormsHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template forms: {ex.Message}");
        }
    }
}