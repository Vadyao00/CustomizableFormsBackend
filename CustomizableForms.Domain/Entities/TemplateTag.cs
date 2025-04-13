namespace CustomizableForms.Domain.Entities;

public class TemplateTag
{
    public Guid TemplateId { get; set; }
    public Template Template { get; set; }
    
    public Guid TagId { get; set; }
    public Tag Tag { get; set; }
}