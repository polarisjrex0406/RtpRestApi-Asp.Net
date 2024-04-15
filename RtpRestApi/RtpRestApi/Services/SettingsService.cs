using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using RtpRestApi.Helpers;
using RtpRestApi.Models;
using System.Text.Json;

namespace RtpRestApi.Services
{
    public class SettingsService
    {
        IAtlasService _atlasService;
        private readonly string _collection;

        public SettingsService(IOptions<RtpServerSettings> rtpDatabaseTopics, IAtlasService atlasService)
        {
            _atlasService = atlasService;
            _collection = rtpDatabaseTopics.Value.SettingsCollectionName;
        }

        public async Task<List<Setting>> GetAsync()
        {
            JArray andArray = new JArray();
            JObject removed = new JObject
            {
                ["removed"] = false
            };
            andArray.Add(removed);

            JObject filterObj = new JObject
            {
                ["$and"] = andArray
            };

            string res = await _atlasService.FindAsync(_collection, filterObj);
            var settingObj = new List<Setting>();
            try
            {
                settingObj = JsonSerializer.Deserialize<List<Setting>>(res);
            }
            catch (Exception)
            {
                return new List<Setting>();
            }

            return settingObj;
        }

        public async Task<Setting?> GetAsync(string id)
        {
            JArray andArray = new JArray();
            JObject removed = new JObject
            {
                ["removed"] = false
            };
            andArray.Add(removed);

            JObject adminId = new JObject
            {
                ["_id"] = new JObject
                {
                    ["$oid"] = id
                }
            };
            andArray.Add(adminId);

            JObject filterObj = new JObject
            {
                ["$and"] = andArray
            };

            string res = await _atlasService.FindAsync(_collection, filterObj);
            var settingObj = new Setting();
            try
            {
                settingObj = JsonSerializer.Deserialize<Setting>(res);
            }
            catch (Exception)
            {
                return new Setting();
            }

            return settingObj;

        }

        public async Task CreateAsync(Setting newSetting)
        {
            string tmp = JsonSerializer.Serialize(newSetting);
            JObject documentObj = JObject.Parse(tmp);
            await _atlasService.InsertOneAsync(_collection, documentObj);
        }

        public async Task UpdateAsync(string id, Setting updatedSetting)
        {
            JObject filterObj = new JObject
            {
                ["_id"] = new JObject
                {
                    ["$oid"] = id
                }
            };
            string tmp = JsonSerializer.Serialize(updatedSetting);
            JObject setObj = JObject.Parse(tmp);

            string res = await _atlasService.UpdateOneAsync(_collection, filterObj, setObj);
            int matchedCount = 0;
            try
            {
                var resObj = JObject.Parse(res);
                var idObj = resObj["matchedCount"];
                if (idObj != null)
                {
                    matchedCount = int.Parse(idObj.ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task RemoveAsync(string id)
        {
            JObject filterObj = new JObject
            {
                ["_id"] = new JObject
                {
                    ["$oid"] = id
                }
            };
            await _atlasService.DeleteOneAsync(_collection, filterObj);
        }
    }
}