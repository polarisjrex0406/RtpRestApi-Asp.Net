using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RtpRestApi.Helpers;
using RtpRestApi.Models;
using System.Text.Json;
using BC = BCrypt.Net.BCrypt;

namespace RtpRestApi.Services
{
    public class AdminPasswordsService
    {
        IAtlasService _atlasService;
        private readonly string _collection;
        public AdminPasswordsService(IOptions<RtpServerSettings> rtpDatabaseTopics, IAtlasService atlasService)
        {
            _atlasService = atlasService;
            _collection = rtpDatabaseTopics.Value.AdminPasswordsCollectionName;
        }

        public bool ValidatePassword(string input, string hash)
        {
            return BC.Verify(input, hash);
        }

        private string GenerateHashPassword(string password)
        {
            return BC.HashPassword(password);
        }

        public async Task<AdminPassword?> GetByUserAsync(string user)
        {
            JArray andArray = new JArray();
            JObject removed = new JObject
            {
                ["removed"] = false
            };
            andArray.Add(removed);

            JObject adminId = new JObject
            {
                ["user"] = new JObject
                {
                    ["$oid"] = user
                }
            };
            andArray.Add(adminId);

            JObject filterObj = new JObject
            {
                ["$and"] = andArray
            };

            string res = await _atlasService.FindOneAsync(_collection, filterObj);
            var adminObj = new AdminPassword();
            try
            {
                adminObj = JsonSerializer.Deserialize<AdminPassword>(res);
            }
            catch (Exception)
            {
                return new AdminPassword();
            }

            return adminObj;
        }

        public async Task CreateAsync(string user, string password, string salt = "")
        {
            AdminPassword adminPassword = new AdminPassword();
            adminPassword.user = user;
            adminPassword.password = GenerateHashPassword(password);
            adminPassword.salt = salt;
            
            string tmp = JsonSerializer.Serialize(adminPassword);
            JObject documentObj = JObject.Parse(tmp);
            documentObj.Remove("_id");
            documentObj["user"] = new JObject
            {
                ["$oid"] = user
            };

            await _atlasService.InsertOneAsync(_collection, documentObj);
        }

        public async Task UpdateAsync(string id, AdminPassword updatedAdminPassword)
        {
            JObject filterObj = new JObject
            {
                ["_id"] = new JObject
                {
                    ["$oid"] = id
                }
            };
            string tmp = JsonSerializer.Serialize(updatedAdminPassword);
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

        public async Task RemoveByUserAsync(string user)
        {
            JObject filterObj = new JObject
            {
                ["user"] = new JObject
                {
                    ["$oid"] = user
                }
            };
            await _atlasService.DeleteOneAsync(_collection, filterObj);
        }

    }
}
