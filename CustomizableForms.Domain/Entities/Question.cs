using System.ComponentModel.DataAnnotations;
using CustomizableForms.Domain.Enums;

namespace CustomizableForms.Domain.Entities;

public class Question
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public int OrderIndex { get; set; }
    
    public bool ShowInResults { get; set; } = true;
    
    public QuestionType Type { get; set; }
    
    public Guid TemplateId { get; set; }
    public Template Template { get; set; }
    
    public ICollection<Answer> Answers { get; set; }
}