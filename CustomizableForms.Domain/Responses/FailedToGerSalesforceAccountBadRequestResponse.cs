namespace CustomizableForms.Domain.Responses;

public class FailedToGerSalesforceAccountBadRequestResponse : ApiBadRequestResponse
{
    public FailedToGerSalesforceAccountBadRequestResponse(string accountError) : base($"Failed to get Salesforce Account data: {accountError}")
    {
    }
}