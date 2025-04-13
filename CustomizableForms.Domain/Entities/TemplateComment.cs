using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.Entities;

public class TemplateComment
{
    public Guid Id { get; set; }
    [Required]
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid TemplateId { get; set; }
    public Template Template { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
}