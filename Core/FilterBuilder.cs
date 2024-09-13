using System.Linq.Expressions;
using TicketingSystem.Common.Models;

namespace TicketingSystem.Core
{
    public static class FilterBuilder
    {
        public static Expression<Func<TicketEntity, bool>> Build(TicketFiltersDto filters)
        {
            Expression<Func<TicketEntity, bool>> expression = product => true;

            //if (!string.IsNullOrEmpty(filters?.Type))
            //{
            //    Console.Write(expression);
            //}

            return expression;
        }
    }
}
