using DryCleaningApi.Models;
using DryCleaningApi.Repositories.Interfaces;
using LR4Premium_Dry_Cleaning_and_Laundry.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DryCleaningApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<User>("Users");
    }

    public async Task<List<User>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<User?> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<User?> GetByEmailAsync(string email) =>
        await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
        await _collection.Find(x => x.RefreshToken == refreshToken).FirstOrDefaultAsync();

    public async Task CreateAsync(User entity) =>
        await _collection.InsertOneAsync(entity);

    public async Task UpdateAsync(string id, User entity) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, entity);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
}