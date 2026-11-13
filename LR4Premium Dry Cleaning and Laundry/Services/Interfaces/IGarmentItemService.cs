using DryCleaningApi.DTOs;

namespace DryCleaningApi.Services.Interfaces;

public interface IGarmentItemService
{
    Task<List<GarmentItemResponseDto>> GetAllAsync();
    Task<GarmentItemResponseDto> GetByIdAsync(string id);
    Task<GarmentItemResponseDto> CreateAsync(CreateGarmentItemDto dto);
    Task<GarmentItemResponseDto> UpdateAsync(string id, UpdateGarmentItemDto dto);
    Task DeleteAsync(string id);
}