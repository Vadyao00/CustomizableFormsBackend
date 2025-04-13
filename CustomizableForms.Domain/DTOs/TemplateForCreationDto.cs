using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.DTOs;

public record TemplateForCreationDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Maximum length for the Title is 200 characters")]
    public string Title { get; init; }
    
    public string Description { get; init; }
    
    [Required(ErrorMessage = "Topic is required")]
    [MaxLength(50, ErrorMessage = "Maximum length for the Topic is 50 characters")]
    public string Topic { get; init; }
    
    public string ImageUrl { get; init; }
    
    public bool IsPublic { get; init; } = false;
    
    public List<string> Tags { get; init; } = new List<string>();
    
    public List<string> AllowedUserEmails { get; init; } = new List<string>();
}