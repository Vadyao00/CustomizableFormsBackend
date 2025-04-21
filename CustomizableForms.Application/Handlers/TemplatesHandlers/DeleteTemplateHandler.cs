using Contracts.IRepositories;
using CustomizableForms.Application.Commands.TemplateCommands;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.TemplatesHandlers;

public class DeleteTemplateHandler(
    IRepositoryManager repository,
    ILoggerManager logger)
    : IRequestHandler<DeleteTemplateCommand, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: true);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }

            bool isAdmin = false;
            var userRoles = await repository.Role.GetUserRolesAsync(request.CurrentUser.Id, trackChanges: false);
            isAdmin = userRoles.Any(r => r.Name == "Admin");

            if (template.CreatorId != request.CurrentUser.Id && !isAdmin)
            {
                return new ApiBadRequestResponse("You do not have permission to delete this template");
            }

            repository.Template.DeleteTemplate(template);
            await repository.SaveAsync();

            return new ApiOkResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(DeleteTemplateHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error deleting template: {ex.Message}");
        }
    }
}