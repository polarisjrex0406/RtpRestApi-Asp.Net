using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using RtpRestApi.Helpers;
using RtpRestApi.Models;
using System.Text.Json;

namespace RtpRestApi.Services
{
    public class AdminsService
    {
        IAtlasService _atlasService;
        private readonly string _collection;
        public AdminsService(IOptions<RtpServerSettings> rtpDatabaseTopics, IAtlasService atlasService)
        {
            _atlasService = atlasService;
            _collection = rtpDatabaseTopics.Value.AdminsCollectionName;
        }

        public async Task<List<Admin>?> GetAsync()
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
            var adminObj = new List<Admin>();
            try
            {
                adminObj = JsonSerializer.Deserialize<List<Admin>>(res);
            }
            catch (Exception)
            {
                return new List<Admin>();
            }

            return adminObj;
        }

        public async Task<Admin?> GetByIdAsync(string id)
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
            var adminObj = new Admin();
            try
            {
                adminObj = JsonSerializer.Deserialize<Admin>(res);
            }
            catch (Exception)
            {
                return new Admin();
            }

            return adminObj;
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            JArray andArray = new JArray();
            JObject removed = new JObject
            {
                ["removed"] = false
            };
            andArray.Add(removed);

            JObject adminId = new JObject
            {
                ["email"] = email
            };
            andArray.Add(adminId);

            JObject filterObj = new JObject
            {
                ["$and"] = andArray
            };

            string res = await _atlasService.FindOneAsync(_collection, filterObj);
            var adminObj = new Admin();
            try
            {
                adminObj = JsonSerializer.Deserialize<Admin>(res);
            }
            catch (Exception)
            {
                return new Admin();
            }

            return adminObj;
        }

        public async Task CreateAsync(Admin newAdmin)
        {

        }

        public async Task UpdateAsync(string id, Admin updatedAdmin)
        {

        }

        public async Task RemoveAsync(string id)
        {

        }
    }
}
