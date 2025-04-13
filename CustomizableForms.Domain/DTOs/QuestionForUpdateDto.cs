using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.DTOs;

public record QuestionForUpdateDto
{
    [Required(ErrorMessage = "Question title is required")]
    [MaxLength(500, ErrorMessage = "Maximum length for the Title is 500 characters")]
    public string Title { get; init; }
    
    public string Description { get; init; }
    
    public bool ShowInResults { get; init; }
}