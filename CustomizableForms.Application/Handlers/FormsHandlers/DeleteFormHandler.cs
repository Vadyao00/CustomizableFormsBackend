using Contracts.IRepositories;
using CustomizableForms.Application.Commands.FormsCommands;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.FormsHandlers;

public class DeleteFormHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<DeleteFormCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(DeleteFormCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var form = await repository.Form.GetFormByIdAsync(request.FormId, trackChanges: true);
            if (form == null)
            {
                return new ApiBadRequestResponse("Form not found");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (form.UserId != request.CurrentUser.Id && form.Template.CreatorId != request.CurrentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to delete this form");
            }

            repository.Form.DeleteForm(form);
            await repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(DeleteFormHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error deleting form: {ex.Message}");
        }
    }
}