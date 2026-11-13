using DryCleaningApi.Models;
namespace DryCleaningApi.Repositories.Interfaces;
public interface IGarmentItemRepository : IRepository<GarmentItem>
{
    Task<List<GarmentItem>> GetByIdsAsync(List<string> ids);
    Task<GarmentItem?> GetByBarcodeAsync(string barcode);
}