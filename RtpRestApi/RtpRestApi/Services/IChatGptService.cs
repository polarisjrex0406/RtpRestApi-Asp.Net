using Newtonsoft.Json.Linq;

namespace RtpRestApi.Services
{
    public interface IChatGptService
    {
        Task<string?> GetChatCompletionAsync(string payload);
    }
}
