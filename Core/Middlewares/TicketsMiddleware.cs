using Newtonsoft.Json.Linq;
using TicketingSystem.Common.Models;

namespace TicketingSystem.Core.Middlewares
{
    public class TicketsMiddleware(RequestDelegate requestDelegate)
    {
        private readonly RequestDelegate _requestDelegate = requestDelegate;

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            using var newBodyStream = new MemoryStream();
            context.Response.Body = newBodyStream;

            await _requestDelegate(context);

            newBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(newBodyStream).ReadToEndAsync();

            var filteredResponseBody = FilterFields(responseBody);

            context.Response.Body = originalBodyStream;
            await context.Response.WriteAsync(filteredResponseBody);
        }


        private static string FilterFields(string body)
        {
            var json = JArray.Parse(body);
            foreach (var item in json)
            {
                string? itemType = (string?)item["type"];

                if (itemType != null)
                {
                    if (itemType != "Epic")
                    {
                        ((JObject)item).Remove("relatedElements");
                    }

                    if (itemType != "Bug")
                    {
                        ((JObject)item).Remove("affectedVersion");
                    }
                } 
            }
            return json.ToString();
        }
    }
}
