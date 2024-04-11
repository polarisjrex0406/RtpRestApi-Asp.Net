using DnsClient;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RtpRestApi.Helpers;
using RtpRestApi.Models;
using System.Net.Http.Headers;
using System.Text;

namespace RtpRestApi.Services
{
    public class AtlasService(IOptions<RtpDatabaseSettings> rtpDatabaseAtlas, IHttpClientFactory httpClientFactory)
        : IAtlasService
    {
        public async Task<string> MakeDataApiCall(string endpointRoute, string collection, JObject? filterObj, JObject? documentObj)
        {
            var httpClient = httpClientFactory.CreateClient("Atlas");

            string baseUrl = rtpDatabaseAtlas.Value.BaseUrl;
            string apiKey = rtpDatabaseAtlas.Value.ApiKey;
            string dataSource = rtpDatabaseAtlas.Value.DataSource;
            string database = rtpDatabaseAtlas.Value.DatabaseName;
            var req = new JObject
            {
                ["collection"] = collection,
                ["database"] = database,
                ["dataSource"] = dataSource
            };
            if (filterObj != null)
            {
                req["filter"] = filterObj;
            }
            if (documentObj != null)
            {
                req["document"] = documentObj;
            }
            string payload = req.ToString();

            using var httpReq = new HttpRequestMessage(HttpMethod.Post, baseUrl + endpointRoute);
            httpReq.Headers.Add("api-key", $"{apiKey}");
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            httpReq.Content = content;

            using HttpResponseMessage? httpResponse = await httpClient.SendAsync(httpReq);
/*            httpResponse.EnsureSuccessStatusCode();*/

            return await httpResponse.Content.ReadAsStringAsync();
        }
        public async Task<string> FindAsync(string collection, JObject filterObj)
        {
            string findRoute = "/action/find";
            string res = await MakeDataApiCall(findRoute, collection, filterObj, null);
            string doc;
            try
            {
                var resObj = JObject.Parse(res);
                var documentObj = resObj["documents"];
                doc = documentObj != null ? documentObj.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                return res;
            }
            
            return doc;
        }
        public async Task<string> FindOneAsync(string collection, JObject filterObj)
        {
            string findOneRoute = "/action/findOne";
            string res = await MakeDataApiCall(findOneRoute, collection, filterObj, null);
            string doc;
            try
            {
                var resObj = JObject.Parse(res);
                var documentObj = resObj["document"];
                doc = documentObj != null ? documentObj.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                return res;
            }

            return doc;
        }
        public Task<string> InsertAsync(string payload)
        {
            return null;
        }
        public async Task<string> InsertOneAsync(string collection, JObject documentObj)
        {
            string insertOneRoute = "/action/insertOne";
            return await MakeDataApiCall(insertOneRoute, collection, null, documentObj);
        }
        public Task<string> UpdateAsync(string payload)
        {
            return null;
        }
        public Task<string> UpdateOneAsync(string payload)
        {
            return null;
        }
        public Task<string> DeleteAsync(string payload)
        {
            return null;
        }
        public async Task<string> DeleteOneAsync(string collection, JObject filterObj)
        {
            string deleteOneRoute = "/action/deleteOne";
            return await MakeDataApiCall(deleteOneRoute, collection, filterObj, null);
        }
    }
}
