using Newtonsoft.Json.Linq;

namespace RtpRestApi.Services
{
    public interface IAtlasService
    {
        Task<string> MakeDataApiCall(string endpointRoute, string collection, JObject? filterObj, JObject? documentObj, JObject? setObj);
        Task<string> FindAsync(string collection, JObject filterObj);
        Task<string> FindOneAsync(string payload, JObject filterObj);
        Task<string> InsertOneAsync(string collection, JObject documentObj);
        Task<string> UpdateOneAsync(string collection, JObject filterObj, JObject setObj);
        Task<string> DeleteOneAsync(string collection, JObject filterObj);
    }
}
