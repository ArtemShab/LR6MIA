using DryCleaningApi.Models;
using DryCleaningApi.Repositories.Interfaces;
using LR4Premium_Dry_Cleaning_and_Laundry.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DryCleaningApi.Repositories;

public class GarmentItemRepository : IGarmentItemRepository
{
    private readonly IMongoCollection<GarmentItem> _collection;

    public GarmentItemRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<GarmentItem>("GarmentItems");
    }

    public async Task<List<GarmentItem>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<GarmentItem?> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<GarmentItem>> GetByIdsAsync(List<string> ids) =>
        await _collection.Find(x => ids.Contains(x.Id!)).ToListAsync();

    public async Task<GarmentItem?> GetByBarcodeAsync(string barcode) =>
        await _collection.Find(x => x.Barcode == barcode).FirstOrDefaultAsync();

    public async Task CreateAsync(GarmentItem entity) =>
        await _collection.InsertOneAsync(entity);

    public async Task UpdateAsync(string id, GarmentItem entity) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, entity);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
}