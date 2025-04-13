namespace CustomizableForms.Persistance.Extensions;

public static class FullTextSearchExtensions
{
    public static string ToTsVector(string text) => $"to_tsvector('russian', {text})";
        
    public static string ToTsQuery(string searchTerm) => $"to_tsquery('russian', '{searchTerm.Replace(" ", "&")}:*')";
        
    public static string CalculateRelevance(string tsVector, string tsQuery) => 
        $"ts_rank({tsVector}, {tsQuery})";
}