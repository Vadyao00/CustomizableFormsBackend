using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CustomizableForms.Domain.DTOs;
using Microsoft.Extensions.Configuration;

namespace CustomizableForms.Application.Handlers.SalesforceHandlers;

public class SalesforceApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        private DateTime _tokenExpirationTime;
        private string _instanceUrl;

        public SalesforceApiHelper(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> AuthenticateAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpirationTime)
                return true;

            var tokenEndpoint = _configuration["Salesforce:TokenEndpoint"];
            var clientId = _configuration["Salesforce:ClientId"];
            var clientSecret = _configuration["Salesforce:ClientSecret"];
            var username = _configuration["Salesforce:Username"];
            var password = _configuration["Salesforce:Password"];

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "username", username },
                { "password", password }
            });

            try
            {
                var response = await _httpClient.PostAsync(tokenEndpoint, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                _accessToken = tokenResponse.AccessToken;
                _instanceUrl = tokenResponse.InstanceUrl;
                _tokenExpirationTime = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 300);
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<(bool Success, string Id, string ErrorMessage)> CreateSObjectAsync<T>(string objectType, T data)
        {
            if (!await AuthenticateAsync())
                return (false, null, "Failed to authenticate with Salesforce");

            var apiVersion = _configuration["Salesforce:ApiVersion"];
            var endpoint = $"{_instanceUrl}/services/data/{apiVersion}/sobjects/{objectType}";

            var content = new StringContent(
                JsonSerializer.Serialize(data), 
                Encoding.UTF8, 
                "application/json"
            );

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var createResponse = JsonSerializer.Deserialize<SalesforceCreateResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return (true, createResponse.Id, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateSObjectAsync<T>(string objectType, string objectId, T data)
        {
            if (!await AuthenticateAsync())
                return (false, "Failed to authenticate with Salesforce");

            var apiVersion = _configuration["Salesforce:ApiVersion"];
            var endpoint = $"{_instanceUrl}/services/data/{apiVersion}/sobjects/{objectType}/{objectId}";

            var content = new StringContent(
                JsonSerializer.Serialize(data), 
                Encoding.UTF8, 
                "application/json"
            );

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            try
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, T Data, string ErrorMessage)> GetSObjectAsync<T>(string objectType, string objectId, string fields)
        {
            if (!await AuthenticateAsync())
                return (false, default, "Failed to authenticate with Salesforce");

            var apiVersion = _configuration["Salesforce:ApiVersion"];
            var endpoint = $"{_instanceUrl}/services/data/{apiVersion}/sobjects/{objectType}/{objectId}?fields={fields}";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return (true, data, null);
            }
            catch (Exception ex)
            {
                return (false, default, ex.Message);
            }
        }
    }