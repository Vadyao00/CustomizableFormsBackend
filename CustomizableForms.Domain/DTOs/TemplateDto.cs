namespace CustomizableForms.Domain.DTOs;

public record TemplateDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Topic { get; init; }
    public string ImageUrl { get; init; }
    public bool IsPublic { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public UserDto Creator { get; init; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int FormsCount { get; set; }
    public List<string> Tags { get; set; }
    public List<string> AllowedUsers { get; set; }
}