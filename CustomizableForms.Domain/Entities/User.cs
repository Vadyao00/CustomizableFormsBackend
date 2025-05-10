

namespace CustomizableForms.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Name { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string PreferredLanguage { get; set; } = "en";
    public string PreferredTheme { get; set; } = "light";
    public bool IsActive { get; set; } = true;
    
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<Template> CreatedTemplates { get; set; }
    public ICollection<Form> SubmittedForms { get; set; }
    public ICollection<TemplateComment> Comments { get; set; }
    public ICollection<TemplateLike> Likes { get; set; }
    public ICollection<TemplateAccess> AccessibleTemplates { get; set; }
    
    public User()
    {
    }
}