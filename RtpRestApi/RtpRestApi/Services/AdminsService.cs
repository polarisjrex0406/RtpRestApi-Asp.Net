using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RtpRestApi.Helpers;
using RtpRestApi.Models;

namespace RtpRestApi.Services
{
    public class AdminsService
    {
        private readonly IMongoCollection<Admin> _adminsCollection;

        public AdminsService(IOptions<RtpDatabaseSettings> rtpDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                rtpDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                rtpDatabaseSettings.Value.DatabaseName);

            _adminsCollection = mongoDatabase.GetCollection<Admin>(
                rtpDatabaseSettings.Value.AdminsCollectionName);
        }

        public async Task<List<Admin>> GetAsync() =>
            await _adminsCollection.Find(_ => true).ToListAsync();

        public async Task<Admin?> GetByIdAsync(string id) =>
            await _adminsCollection.Find(x => x._id == id).FirstOrDefaultAsync();

        public async Task<Admin?> GetByEmailAsync(string email) =>
            await _adminsCollection.Find(x => x.email == email).FirstOrDefaultAsync();

        public async Task CreateAsync(Admin newAdmin) =>
            await _adminsCollection.InsertOneAsync(newAdmin);

        public async Task UpdateAsync(string id, Admin updatedAdmin) =>
            await _adminsCollection.ReplaceOneAsync(x => x._id == id, updatedAdmin);

        public async Task RemoveAsync(string id) =>
            await _adminsCollection.DeleteOneAsync(x => x._id == id);
    }
}
