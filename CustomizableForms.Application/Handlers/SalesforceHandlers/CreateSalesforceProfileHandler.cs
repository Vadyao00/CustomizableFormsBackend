using Contracts.IRepositories;
using CustomizableForms.Application.Commands.SalesforceCommands;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace CustomizableForms.Application.Handlers.SalesforceHandlers;

public class CreateSalesforceProfileHandler : IRequestHandler<CreateSalesforceProfileCommand, ApiBaseResponse>
{
    private readonly IRepositoryManager _repository;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public CreateSalesforceProfileHandler(
        IRepositoryManager repository,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _repository = repository;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ApiBaseResponse> Handle(CreateSalesforceProfileCommand request, CancellationToken cancellationToken)
    {
        var existingProfile = await _repository.UserSalesforceProfile.GetByUserIdAsync(request.UserId, false);
            
        if (existingProfile != null)
            return new SalesforceProfileAlreadyExistsBadRequestResponse();

        var apiHelper = new SalesforceApiHelper(_httpClient, _configuration);

        var accountData = new
        {
            Name = request.ProfileData.CompanyName,
            Website = request.ProfileData.Website,
            Industry = request.ProfileData.Industry,
            Description = request.ProfileData.Description,
            Phone = request.ProfileData.CompanyPhone
        };

        var (accountSuccess, accountId, accountError) = await apiHelper.CreateSObjectAsync("Account", accountData);
        if (!accountSuccess)
            return new FailedToCreateProfileBadRequestResponse(accountError);

        var contactData = new
        {
            FirstName = request.ProfileData.FirstName,
            LastName = request.ProfileData.LastName,
            Email = request.ProfileData.Email,
            Phone = request.ProfileData.Phone,
            Title = request.ProfileData.Title,
            AccountId = accountId
        };

        var (contactSuccess, contactId, contactError) = await apiHelper.CreateSObjectAsync("Contact", contactData);
        if (!contactSuccess)
            return new FailedToCreateProfileBadRequestResponse(accountError);

        var userSalesforceProfile = new UserSalesforceProfile
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            SalesforceAccountId = accountId,
            SalesforceContactId = contactId,
            CreatedAt = DateTime.UtcNow
        };

        _repository.UserSalesforceProfile.CreateProfile(userSalesforceProfile);
        await _repository.SaveAsync();

        return new ApiOkResponse<UserSalesforceProfile>(userSalesforceProfile);
    }
}