namespace CustomizableForms.Domain.Responses;

public class BlockedUserBadRequestResponse : ApiBadRequestResponse
{
    public BlockedUserBadRequestResponse() : base("Your account is blocked")
    {
    }
}