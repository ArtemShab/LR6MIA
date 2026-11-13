using DryCleaningApi.Models;
namespace DryCleaningApi.Repositories.Interfaces;
public interface ICleanClientRepository : IRepository<CleanClient>
{
    Task<CleanClient?> GetByPhoneAsync(string phoneNumber);
}