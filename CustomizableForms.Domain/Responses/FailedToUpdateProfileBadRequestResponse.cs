namespace CustomizableForms.Domain.Responses;

public class FailedToUpdateProfileBadRequestResponse : ApiBadRequestResponse
{
    public FailedToUpdateProfileBadRequestResponse(string message) : base($"Failed to update Salesforce Account: {message}")
    {
    }
}