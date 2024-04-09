using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RtpRestApi.Helpers;
using RtpRestApi.Models;

namespace RtpRestApi.Services
{
    public class SettingsService
    {
        private readonly IMongoCollection<Setting> _settingsCollection;

        public SettingsService(IOptions<RtpDatabaseSettings> rtpDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                rtpDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                rtpDatabaseSettings.Value.DatabaseName);

            _settingsCollection = mongoDatabase.GetCollection<Setting>(
                rtpDatabaseSettings.Value.SettingsCollectionName);
        }

        public async Task<List<Setting>> GetAsync() =>
            await _settingsCollection.Find(_ => true).ToListAsync();

        public async Task<Setting?> GetAsync(string id) =>
            await _settingsCollection.Find(x => x._id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Setting newSetting) =>
            await _settingsCollection.InsertOneAsync(newSetting);

        public async Task UpdateAsync(string id, Setting updatedSetting) =>
            await _settingsCollection.ReplaceOneAsync(x => x._id == id, updatedSetting);

        public async Task RemoveAsync(string id) =>
            await _settingsCollection.DeleteOneAsync(x => x._id == id);
    }
}
