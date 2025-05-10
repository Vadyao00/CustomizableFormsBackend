namespace CustomizableForms.Domain.Responses;

public class SalesforceProfileAlreadyExistsBadRequestResponse : ApiBadRequestResponse
{
    public SalesforceProfileAlreadyExistsBadRequestResponse() : base("A Salesforce profile already exists for this user")
    {
    }
}