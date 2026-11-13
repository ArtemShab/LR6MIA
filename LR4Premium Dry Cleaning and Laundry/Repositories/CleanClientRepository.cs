using DryCleaningApi.Models;
using DryCleaningApi.Repositories.Interfaces;
using LR4Premium_Dry_Cleaning_and_Laundry.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DryCleaningApi.Repositories;

public class CleanClientRepository : ICleanClientRepository
{
    private readonly IMongoCollection<CleanClient> _collection;

    public CleanClientRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<CleanClient>("CleanClients");
    }

    public async Task<List<CleanClient>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<CleanClient?> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<CleanClient?> GetByPhoneAsync(string phoneNumber) =>
        await _collection.Find(x => x.PhoneNumber == phoneNumber).FirstOrDefaultAsync();

    public async Task CreateAsync(CleanClient entity) =>
        await _collection.InsertOneAsync(entity);

    public async Task UpdateAsync(string id, CleanClient entity) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, entity);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
}