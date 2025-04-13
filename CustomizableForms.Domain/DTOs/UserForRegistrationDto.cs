using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.DTOs;

public record UserForRegistrationDto
{
    [Required(ErrorMessage = "Username is required")]
    public string? Name { get; init; }
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; init; }
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; init; }
}