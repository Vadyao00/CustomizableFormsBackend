namespace CustomizableForms.Domain.Responses;

public class BadUserBadRequestResponse : ApiBadRequestResponse
{
    public BadUserBadRequestResponse(): base("You deleted or blocked.")
    {
    }
}