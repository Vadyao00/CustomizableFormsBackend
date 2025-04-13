using System.ComponentModel.DataAnnotations;

namespace CustomizableForms.Domain.DTOs;

public record UserDto
{
    public string? Id { get; set; }
    
    [Display(Name = "Логин")]
    public string? Name { get; set; }
    
    [EmailAddress(ErrorMessage = "Некорректный адрес")]
    public string? Email { get; set; }
    
    [Display(Name = "Статус")]
    public string? Status { get; set; }
    
    public List<string>? Roles { get; set; }
}