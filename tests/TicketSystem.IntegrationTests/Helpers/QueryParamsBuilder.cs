using System.Web;

namespace TicketSystem.IntegrationTests.Helpers
{
    public class QueryParamsBuilder
    {
        public string BuildQueryParams<T>(T dto)
        {
            var properties = typeof(T).GetProperties();
            var queryParams = new List<string>();

            foreach (var property in properties)
            {
                var value = property.GetValue(dto);
                if (value != null)
                {
                    string key = HttpUtility.UrlEncode(property.Name);
                    string val = HttpUtility.UrlEncode(value.ToString());
                    queryParams.Add($"{key}={val}");
                }
            }

            return string.Join("&", queryParams);
        }
    }
}
