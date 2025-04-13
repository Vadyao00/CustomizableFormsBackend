namespace CustomizableForms.Domain.Entities;

public class Form
{
    public Guid Id { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    public Guid TemplateId { get; set; }
    public Template Template { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public ICollection<Answer> Answers { get; set; }
}