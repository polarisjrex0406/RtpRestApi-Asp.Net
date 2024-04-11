using Newtonsoft.Json.Linq;

namespace RtpRestApi.Services
{
    public interface IAtlasService
    {
        Task<string> MakeDataApiCall(string endpointRoute, string collection, JObject? filterObj, JObject? documentObj);
        Task<string> FindAsync(string collection, JObject filterObj);
        Task<string> FindOneAsync(string payload, JObject filterObj);
        Task<string> InsertAsync(string payload);
        Task<string> InsertOneAsync(string collection, JObject documentObj);
        Task<string> UpdateAsync(string payload);
        Task<string> UpdateOneAsync(string payload);
        Task<string> DeleteAsync(string payload);
        Task<string> DeleteOneAsync(string collection, JObject filterObj);
    }
}
