namespace CustomizableForms.Domain.Entities;

public class UserSalesforceProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SalesforceAccountId { get; set; }
    public string SalesforceContactId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}