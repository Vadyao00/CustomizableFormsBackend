using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    public ICollection<TemplateTag> TemplateTags { get; set; }
}