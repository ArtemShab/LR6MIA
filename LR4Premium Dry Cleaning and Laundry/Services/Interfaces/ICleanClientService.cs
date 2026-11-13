using DryCleaningApi.DTOs;

namespace DryCleaningApi.Services.Interfaces;

public interface ICleanClientService
{
    Task<List<CleanClientResponseDto>> GetAllAsync();
    Task<CleanClientResponseDto> GetByIdAsync(string id);
    Task<CleanClientResponseDto> CreateAsync(CreateCleanClientDto dto);
    Task<CleanClientResponseDto> UpdateAsync(string id, UpdateCleanClientDto dto);
    Task DeleteAsync(string id);
}