using AutoMapper;
using DryCleaningApi.DTOs;
using DryCleaningApi.Models;
using DryCleaningApi.Repositories.Interfaces;
using DryCleaningApi.Services.Interfaces;

namespace DryCleaningApi.Services;

public class GarmentItemService : IGarmentItemService
{
    private readonly IGarmentItemRepository _repository;
    private readonly IMapper _mapper;

    public GarmentItemService(IGarmentItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<GarmentItemResponseDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return _mapper.Map<List<GarmentItemResponseDto>>(items);
    }

    public async Task<GarmentItemResponseDto> GetByIdAsync(string id)
    {
        var item = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Річ з ID {id} не знайдено.");
        return _mapper.Map<GarmentItemResponseDto>(item);
    }

    public async Task<GarmentItemResponseDto> CreateAsync(CreateGarmentItemDto dto)
    {
        if (dto.WearPercentage < 10 || dto.WearPercentage > 50)
            throw new ArgumentException("Відсоток зносу речі повинен бути в межах від 10% до 50%.");

        var entity = _mapper.Map<GarmentItem>(dto);
        entity.Barcode = $"RFID-{Guid.NewGuid().ToString("N")[..10].ToUpper()}";
        await _repository.CreateAsync(entity);
        return _mapper.Map<GarmentItemResponseDto>(entity);
    }

    public async Task<GarmentItemResponseDto> UpdateAsync(string id, UpdateGarmentItemDto dto)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Річ з ID {id} не знайдено.");

        if (dto.WearPercentage < 10 || dto.WearPercentage > 50)
            throw new ArgumentException("Відсоток зносу речі повинен бути в межах від 10% до 50%.");

        var updated = _mapper.Map<GarmentItem>(dto);
        updated.Id = existing.Id;
        updated.Barcode = existing.Barcode;
        await _repository.UpdateAsync(id, updated);
        return _mapper.Map<GarmentItemResponseDto>(updated);
    }

    public async Task DeleteAsync(string id)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Річ з ID {id} не знайдено.");
        await _repository.DeleteAsync(id);
    }
}