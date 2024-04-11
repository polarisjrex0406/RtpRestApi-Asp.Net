using Newtonsoft.Json.Linq;
using RtpRestApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RtpRestApi.Services
{
    public class ExperimentService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        : IExperimentService
    {

        public async Task<List<ExperimentResponse>> GetExperimentAsync()
        {
            var httpClient = httpClientFactory.CreateClient("Atlas");

            var req = new JObject
            {
                ["collection"] = "experiments",
                ["database"] = "test",
                ["dataSource"] = "RTPServerlessInstance",
                ["filter"] = new JObject
                {
                    ["removed"] = false,
                    ["createdBy"] = new JObject
                    {
                        ["$oid"] = "660464961448739672728709"
                    }
                }
            };

            var atlasRequest = new
            {
                collection = "experiments",
                database = "test",
                dataSource = "RTPServerlessInstance",
                filter = new
                {
                    removed = false
                }
            };
            

            using var httpReq = new HttpRequestMessage(HttpMethod.Post, "https://eu-central-1.aws.data.mongodb-api.com/app/data-jpxcz/endpoint/data/v1/action/find");
            httpReq.Headers.Add("api-key", $"g9nhE9ffDexKWgJdLpaMNaQ8f5B5gtDeWjMw16f7RuFhSAoqT421xQHU49m9AwAs");

            /*            string requestString = JsonSerializer.Serialize(atlasRequest);*/
            string requestString = req.ToString();
            httpReq.Content = new StringContent(requestString, Encoding.UTF8, "application/json");

            using HttpResponseMessage? httpResponse = await httpClient.SendAsync(httpReq);
            httpResponse.EnsureSuccessStatusCode();

            if (!httpResponse.IsSuccessStatusCode)
            {
                return null;
            }

            string res = await httpResponse.Content.ReadAsStringAsync();

            /*            var experimentResponse = httpResponse.IsSuccessStatusCode ? JsonSerializer.Deserialize<ExperimentResponse>(await httpResponse.Content.ReadAsStringAsync()) : null;*/
            JObject resObj = JObject.Parse(res);
            var experimentResponse = JsonSerializer.Deserialize<List<ExperimentResponse>>(resObj["documents"].ToString());
            return experimentResponse;
        }
    }
}
