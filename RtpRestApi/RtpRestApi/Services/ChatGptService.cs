using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Amazon.Runtime;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using RtpRestApi.Helpers;
using RtpRestApi.Models;
using System.Text.Json;

namespace RtpRestApi.Services
{
    public class ChatGptService(IOptions<RtpServerSettings> rtpDatabaseAtlas, 
        IHttpClientFactory httpClientFactory)
    {
        public async Task<string?> GetChatCompletionAsync(string payload)
        {
            string uri = rtpDatabaseAtlas.Value.OpenAIUri;
            string apiKey = rtpDatabaseAtlas.Value.OpenAIKey;

            var httpClient = httpClientFactory.CreateClient("OpenAI");
            httpClient.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var httpReq = new HttpRequestMessage(HttpMethod.Post, uri);
            httpReq.Headers.Add("Authorization", $"Bearer {apiKey}");
            httpReq.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            httpReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            string content = string.Empty;
            await httpClient.SendAsync(httpReq)
                    .ContinueWith(async responseTask =>
                    {
                        Console.WriteLine("Response: {0}", responseTask.Result);
                        content = await responseTask.Result.Content.ReadAsStringAsync();
                    });

            var completionResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(content);
            if (completionResponse != null && completionResponse?.Choices != null)
            {
                return completionResponse?.Choices?[0]?.Message?.Content;
            }
            else
            {
                var errorMsg = JsonSerializer.Deserialize<ErrorResponse>(content);
                return errorMsg?.GptError?.ErrMessage ?? string.Empty;
            }
        }
    }
}
