using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.Entities;

public class Template
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    public string Description { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Topic { get; set; }
    public string ImageUrl { get; set; }
    public bool IsPublic { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public Guid CreatorId { get; set; }
    public User Creator { get; set; }
    
    public ICollection<Question> Questions { get; set; }
    public ICollection<TemplateTag> TemplateTags { get; set; }
    public ICollection<Form> Forms { get; set; }
    public ICollection<TemplateComment> Comments { get; set; }
    public ICollection<TemplateLike> Likes { get; set; }
    public ICollection<TemplateAccess> AllowedUsers { get; set; }
}