using Contracts.IRepositories;
using CustomizableForms.Application.Queries.SalesforceQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace CustomizableForms.Application.Handlers.SalesforceHandlers;

public class GetSalesforceProfileDataHandler : IRequestHandler<GetSalesforceProfileDataQuery, ApiBaseResponse>
{
    private readonly IRepositoryManager _repository;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GetSalesforceProfileDataHandler(
        IRepositoryManager repository,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _repository = repository;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ApiBaseResponse> Handle(GetSalesforceProfileDataQuery request, CancellationToken cancellationToken)
    {
        var profile = await _repository.UserSalesforceProfile.GetByUserIdAsync(request.UserId, false);

        if (profile == null)
            return new NoProfileFoundBadRequestResponse();

        var apiHelper = new SalesforceApiHelper(_httpClient, _configuration);

        var accountFields = "Name,Website,Industry,Description,Phone";
        var (accountSuccess, accountData, accountError) = await apiHelper.GetSObjectAsync<SalesforceAccount>(
            "Account", 
            profile.SalesforceAccountId, 
            accountFields
        );

        if (!accountSuccess)
            return new FailedToGerSalesforceAccountBadRequestResponse(accountError);

        var contactFields = "FirstName,LastName,Email,Phone,Title";
        var (contactSuccess, contactData, contactError) = await apiHelper.GetSObjectAsync<SalesforceContact>(
            "Contact", 
            profile.SalesforceContactId, 
            contactFields
        );
        
        if (!contactSuccess)
            return new FailedToGerSalesforceAccountBadRequestResponse(accountError);

        var profileData = new SalesforceProfileDto
        {
            CompanyName = accountData.Name,
            Website = accountData.Website,
            Industry = accountData.Industry,
            Description = accountData.Description,
            CompanyPhone = accountData.Phone,
            
            FirstName = contactData.FirstName,
            LastName = contactData.LastName,
            Email = contactData.Email,
            Phone = contactData.Phone,
            Title = contactData.Title
        };

        return new ApiOkResponse<SalesforceProfileDto>(profileData);
    }
}
public class SalesforceAccount
{
    public string Name { get; set; }
    public string Website { get; set; }
    public string Industry { get; set; }
    public string Description { get; set; }
    public string Phone { get; set; }
}

public class SalesforceContact
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Title { get; set; }
}