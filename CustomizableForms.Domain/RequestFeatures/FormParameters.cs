namespace CustomizableForms.Domain.RequestFeatures;

public class FormParameters : RequestParameters
{
    public FormParameters() => OrderBy = "SubmittedAt";
}