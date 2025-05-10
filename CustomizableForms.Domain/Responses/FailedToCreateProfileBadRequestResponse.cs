namespace CustomizableForms.Domain.Responses;

public class FailedToCreateProfileBadRequestResponse : ApiBadRequestResponse
{
    public FailedToCreateProfileBadRequestResponse(string message) : base($"Failed to create Salesforce Account: {message}")
    {
    }
}