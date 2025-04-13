using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.DTOs;

public record CommentForCreationDto
{
    [Required(ErrorMessage = "Comment content is required")]
    public string Content { get; init; }
}