using DryCleaningApi.Models;

namespace DryCleaningApi.Repositories.Interfaces;

public interface ICleaningReceiptRepository : IRepository<CleaningReceipt>
{
    Task<CleaningReceipt?> GetByReceiptNumberAsync(string receiptNumber);
    Task<List<CleaningReceipt>> GetByClientPhoneAsync(string phone);
}