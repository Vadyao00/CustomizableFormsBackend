using CustomizableForms.Domain.Entities;
using CustomizableForms.Persistance.Extensions.Utility;
using System.Linq.Dynamic.Core;

namespace CustomizableForms.Persistance.Extensions;

public static class RepositoryTemplateExtensions
{
    public static IQueryable<Template> Search(this IQueryable<Template> templates, string searchName)
    {
        if(string.IsNullOrWhiteSpace(searchName))
            return templates;

        var lowerCaseName = searchName.Trim().ToLower();

        return templates.Where(t => t.Topic.ToLower() == lowerCaseName);
    }
    
    public static IQueryable<Template> Sort(this IQueryable<Template> templates, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return templates.OrderBy(e => e.Title);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Template>(orderByQueryString);

        if (string.IsNullOrWhiteSpace(orderQuery))
            return templates.OrderBy(e => e.Title);

        return templates.OrderBy(orderQuery);
    }
}