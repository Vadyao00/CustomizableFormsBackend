namespace CustomizableForms.Domain.DTOs;

public class SalesforceProfileStatusDto
{
    public bool Exists { get; set; }
    public string AccountId { get; set; }
    public string ContactId { get; set; }
    public DateTime? CreatedAt { get; set; }
}