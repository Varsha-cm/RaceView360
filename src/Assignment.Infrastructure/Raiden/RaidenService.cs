using Newtonsoft.Json.Linq;
using Assignment.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Assignment.Infrastructure.Raiden
{
    public class RaidenService : IRaidenService
    {
        public async Task<string> ApplicationAuthentication()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("RAIDEN_API_URL"));
            request.Headers.Add("accept", "*/*");
            var content = new StringContent("{\r\n  \"clientId\": \"" + Environment.GetEnvironmentVariable("RaidenClientId") + "\",\r\n  \"clientSecret\": \"" + Environment.GetEnvironmentVariable("RaidenClientSecret") + "\"\r\n}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var fetchedResponse = await response.Content.ReadAsStringAsync();
            var JsonToken = JObject.Parse(fetchedResponse);
            var token = JsonToken["token"].ToString();
            return token;
        }
    }
}