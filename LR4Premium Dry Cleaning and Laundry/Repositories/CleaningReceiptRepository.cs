using DryCleaningApi.Models;
using DryCleaningApi.Repositories.Interfaces;
using LR4Premium_Dry_Cleaning_and_Laundry.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DryCleaningApi.Repositories;

public class CleaningReceiptRepository : ICleaningReceiptRepository
{
    private readonly IMongoCollection<CleaningReceipt> _collection;

    public CleaningReceiptRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<CleaningReceipt>("CleaningReceipts");
    }

    public async Task<List<CleaningReceipt>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<CleaningReceipt?> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<CleaningReceipt?> GetByReceiptNumberAsync(string receiptNumber) =>
        await _collection.Find(x => x.ReceiptNumber == receiptNumber).FirstOrDefaultAsync();

    public async Task<List<CleaningReceipt>> GetByClientPhoneAsync(string phone) =>
        await _collection.Find(x => x.ClientPhone == phone).ToListAsync();

    public async Task CreateAsync(CleaningReceipt entity) =>
        await _collection.InsertOneAsync(entity);

    public async Task UpdateAsync(string id, CleaningReceipt entity) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, entity);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
}