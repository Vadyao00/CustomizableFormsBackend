namespace CustomizableForms.Domain.Entities;

public class TemplateLike
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid TemplateId { get; set; }
    public Template Template { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
}