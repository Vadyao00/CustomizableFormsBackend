namespace CustomizableForms.Domain.Entities;

public class TemplateAccess
{
    public Guid TemplateId { get; set; }
    public Template Template { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
}
