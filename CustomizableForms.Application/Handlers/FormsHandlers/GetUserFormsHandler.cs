using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.FormsQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using MediatR;

namespace CustomizableForms.Application.Handlers.FormsHandlers;

public class GetUserFormsHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetUserFormsQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetUserFormsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var forms = await repository.Form.GetUserFormsAsync(request.FormParameters, request.CurrentUser.Id, trackChanges: false);
            var formsDto = mapper.Map<IEnumerable<FormDto>>(forms);

            return new ApiOkResponse<(IEnumerable<FormDto>, MetaData)>((formsDto, forms.MetaData));
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in {nameof(GetUserFormsHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving user forms: {ex.Message}");
        }
    }
}