using System.Reflection;
using TicketingSystem.Common.Models;
using TicketingSystem.Core;

public static class QueryExtensions
{
    public static IQueryable<T> ApplyFilter<T>(this IQueryable<TicketEntity> query, TicketFiltersDto filters)
    {
        var predicate = PredicateBuilder.True<TicketEntity>();

        var properties = filters?.GetType().GetProperties() ?? [];
        foreach(PropertyInfo prop in properties)
        {
            var key = prop.Name;
            var value = prop.GetValue(filters);

            if (key == "Type" && value != null)
            {
                predicate = predicate.And(p => p.Type == value.ToString());
            }

            if (key == "Assignee" && value != null)
            {
                predicate = predicate.And(p => p.Assignee.ToString() == value.ToString());
            }

            if (key == "Status" && value != null)
            {
                predicate = predicate.And(p => p.Status == value.ToString());
            }

            if (key == "AffectedVersion" && value != null)
            {
                predicate = predicate.And(p => p.AffectedVersion == value.ToString());
            }
        }

        return (IQueryable<T>)query.Where(predicate);
    }
}