namespace CustomizableForms.Domain.Responses;

public class NoProfileFoundBadRequestResponse : ApiBadRequestResponse
{
    public NoProfileFoundBadRequestResponse() : base("No Salesforce profile found for this user")
    {
    }
}