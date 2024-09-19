using System.Reflection;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Models;

namespace TicketingSystem.Core.Database
{
    public static class QueryExtensions
    {
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<TicketEntity> query, TicketFiltersDto filters)
        {
            var predicate = PredicateBuilder.True<TicketEntity>();

            var properties = filters?.GetType().GetProperties() ?? [];
            foreach (PropertyInfo prop in properties)
            {
                var key = prop.Name;
                var value = prop.GetValue(filters);

                if (key == "Type" && value != null)
                {
                    TicketTypeEnum enumValue = (TicketTypeEnum)Enum.Parse(typeof(TicketTypeEnum), (string)value, true);

                    predicate = predicate.And(p => (int)p.Type == (int)enumValue);
                }

                if (key == "Assignee" && value != null)
                {
                    predicate = predicate.And(p => p.Assignee.ToString() == value.ToString());
                }

                if (key == "Status" && value != null)
                {
                    TicketStatusEnum enumValue = (TicketStatusEnum)Enum.Parse(typeof(TicketStatusEnum), (string)value, true);

                    predicate = predicate.And(p => (int)p.Status == (int)enumValue);
                }

                if (key == "AffectedVersion" && value != null)
                {
                    predicate = predicate.And(p => p.AffectedVersion == value.ToString());
                }
            }

            return (IQueryable<T>)query.Where(predicate);
        }
    }
}