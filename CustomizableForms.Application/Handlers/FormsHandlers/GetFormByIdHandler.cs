using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.FormsQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.FormsHandlers;

public class GetFormByIdHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetFormByIdQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetFormByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var form = await repository.Form.GetFormByIdAsync(request.FormId, trackChanges: false);
            if (form == null)
            {
                return new ApiBadRequestResponse("Form not found");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (form.UserId != request.CurrentUser.Id && form.Template.CreatorId != request.CurrentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to view this form");
            }

            var formDto = mapper.Map<FormDto>(form);
            return new ApiOkResponse<FormDto>(formDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetFormByIdHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving form: {ex.Message}");
        }
    }
}