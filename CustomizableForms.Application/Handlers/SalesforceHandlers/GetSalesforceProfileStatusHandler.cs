using Contracts.IRepositories;
using CustomizableForms.Application.Queries.SalesforceQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Handlers.SalesforceHandlers;

public class GetSalesforceProfileStatusHandler : IRequestHandler<GetSalesforceProfileStatusQuery, ApiBaseResponse>
{
    private readonly IRepositoryManager _repository;

    public GetSalesforceProfileStatusHandler(IRepositoryManager repository)
    {
        _repository = repository;
    }

    public async Task<ApiBaseResponse> Handle(GetSalesforceProfileStatusQuery request, CancellationToken cancellationToken)
    {
        var profile = await _repository.UserSalesforceProfile.GetByUserIdAsync(request.UserId, false);

        var status = new SalesforceProfileStatusDto
        {
            Exists = profile != null,
            AccountId = profile?.SalesforceAccountId,
            ContactId = profile?.SalesforceContactId,
            CreatedAt = profile?.CreatedAt
        };

        return new ApiOkResponse<SalesforceProfileStatusDto>(status);
    }
}
