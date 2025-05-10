namespace CustomizableForms.Domain.ConfigurationModels;

public class SalesforceSettings
{
    public string TokenEndpoint { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ApiVersion { get; set; }
}