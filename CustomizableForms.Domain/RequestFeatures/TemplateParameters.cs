namespace CustomizableForms.Domain.RequestFeatures;

public class TemplateParameters : RequestParameters
{
    public TemplateParameters() => OrderBy = "Title";
    public string? searchTopic { get; set; }
}