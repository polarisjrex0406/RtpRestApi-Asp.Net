﻿using Microsoft.Extensions.Options;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using RtpRestApi.Helpers;
using RtpRestApi.Models;
using MongoDB.Driver.Linq;

namespace RtpRestApi.Services
{
    public class CachePromptsService
    {
        IAtlasService _atlasService;
        private readonly string _collection;

        public CachePromptsService(IOptions<RtpServerSettings> rtpDatabaseTopics, IAtlasService atlasService)
        {
            _atlasService = atlasService;
            _collection = rtpDatabaseTopics.Value.CachePromptsCollectionName;
        }

        private DateTime CalculateExpired(string? cacheTimeoutUnit, int? cacheTimeoutValue)
        {
            DateTime expired = DateTime.Now;
            if (cacheTimeoutUnit == "months" && cacheTimeoutValue != null)
            {
                expired = expired.AddMonths((int)cacheTimeoutValue);
            }
            else if (cacheTimeoutUnit == "weeks" && cacheTimeoutValue != null)
            {
                expired = expired.AddWeeks((int)cacheTimeoutValue);
            }
            else if (cacheTimeoutUnit == "days" && cacheTimeoutValue != null)
            {
                expired = expired.AddDays((double)cacheTimeoutValue);
            }
            else if (cacheTimeoutUnit == "hours" && cacheTimeoutValue != null)
            {
                expired = expired.AddHours((double)cacheTimeoutValue);
            }
            else if (cacheTimeoutUnit == "minutes" && cacheTimeoutValue != null)
            {
                expired = expired.AddMinutes((double)cacheTimeoutValue);
            }
            else if (cacheTimeoutUnit == "seconds" && cacheTimeoutValue != null)
            {
                expired = expired.AddSeconds((double)cacheTimeoutValue);
            }

            return expired;
        }
        public async Task<CachePromptResponse?> CreateOneAsync(CachePromptRequest newCacheRequest, string? adminId)
        {
            CachePromptResponse cacheResponse = new CachePromptResponse();
            cacheResponse.template = newCacheRequest.template;
            cacheResponse.cacheConditions = newCacheRequest.cacheConditions;
            cacheResponse.chatgptSettings = newCacheRequest.chatgptSettings;
            cacheResponse.input = newCacheRequest.input;
            cacheResponse.output = newCacheRequest.output;
            cacheResponse.expired = CalculateExpired(newCacheRequest.cacheTimeoutUnit, newCacheRequest.cacheTimeoutValue);
            cacheResponse.createdBy = adminId;

            string tmp = JsonSerializer.Serialize(cacheResponse);
            JObject documentObj = JObject.Parse(tmp);
            documentObj.Remove("_id");

            string res = await _atlasService.InsertOneAsync(_collection, documentObj);
            try
            {
                var resObj = JObject.Parse(res);
                var idObj = resObj["insertedId"];
                cacheResponse._id = idObj?.ToString();
            }
            catch (Exception)
            {
                return null;
            }

            return cacheResponse;
        }
        public async Task<CachePromptResponse?> ReadOneByArtifactAsync(string? templateId, string? adminId, List<ChatGPTSetting> gptSettings, string enhancedPrompt)
        {
            DateTime? dateTimeNow = DateTime.Now;
            JObject filterObj = new JObject();
            filterObj["removed"] = false;
            filterObj["createdBy"] = adminId;
            filterObj["template"] = templateId;
            if (gptSettings != null)
            {
                string tmp = JsonSerializer.Serialize(gptSettings);
                JArray gptObject = JArray.Parse(tmp);
                filterObj["chatgptSettings"] = gptObject;
            }
            else
            {
                filterObj["chatgptSettings"] = null;
            }
            filterObj["input"] = enhancedPrompt;
            // Check not expired.
            filterObj["expired"] = new JObject
            {
                ["$gt"] = dateTimeNow
            };
            // Get cached prompt and its response.
            string res = await _atlasService.FindOneAsync(_collection, filterObj);
            var cacheObj = new CachePromptResponse();
            try
            {
                cacheObj = JsonSerializer.Deserialize<CachePromptResponse>(res);
            }
            catch (Exception)
            {
                return null;
            }

            return cacheObj;
        }
        public void UpdateOneAsync()
        {
        }
        public void RemoveOneAsync()
        {
        }
    }
}
