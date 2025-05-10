using Contracts.IRepositories;
using CustomizableForms.Application.Commands.SalesforceCommands;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace CustomizableForms.Application.Handlers.SalesforceHandlers;

public class UpdateSalesforceProfileHandler : IRequestHandler<UpdateSalesforceProfileCommand, ApiBaseResponse>
    {
        private readonly IRepositoryManager _repository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UpdateSalesforceProfileHandler(
            IRepositoryManager repository,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _repository = repository;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ApiBaseResponse> Handle(UpdateSalesforceProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _repository.UserSalesforceProfile.GetByUserIdAsync(request.UserId, true);
                
            if (profile == null)
                return new NoProfileFoundBadRequestResponse();

            var apiHelper = new SalesforceApiHelper(_httpClient, _configuration);

            var accountData = new
            {
                Name = request.ProfileData.CompanyName,
                Website = request.ProfileData.Website,
                Industry = request.ProfileData.Industry,
                Description = request.ProfileData.Description,
                Phone = request.ProfileData.CompanyPhone
            };

            var (accountSuccess, accountError) = await apiHelper.UpdateSObjectAsync(
                "Account", 
                profile.SalesforceAccountId, 
                accountData
            );
            
            if (!accountSuccess)
                return new FailedToUpdateProfileBadRequestResponse(accountError);

            var contactData = new
            {
                FirstName = request.ProfileData.FirstName,
                LastName = request.ProfileData.LastName,
                Email = request.ProfileData.Email,
                Phone = request.ProfileData.Phone,
                Title = request.ProfileData.Title
            };

            var (contactSuccess, contactError) = await apiHelper.UpdateSObjectAsync(
                "Contact", 
                profile.SalesforceContactId, 
                contactData
            );
            
            if (!contactSuccess)
                return new FailedToUpdateProfileBadRequestResponse(accountError);

            profile.UpdatedAt = DateTime.UtcNow;
            _repository.UserSalesforceProfile.UpdateProfile(profile);
            await _repository.SaveAsync();

            return new ApiOkResponse<UserSalesforceProfile>(profile);
        }
    }