using AutoMapper;
using DryCleaningApi.DTOs;
using DryCleaningApi.Models;
using DryCleaningApi.Repositories.Interfaces;
using DryCleaningApi.Services.Interfaces;

namespace DryCleaningApi.Services;

public class CleanClientService : ICleanClientService
{
    private readonly ICleanClientRepository _repository;
    private readonly IMapper _mapper;

    public CleanClientService(ICleanClientRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<CleanClientResponseDto>> GetAllAsync()
    {
        var clients = await _repository.GetAllAsync();
        return _mapper.Map<List<CleanClientResponseDto>>(clients);
    }

    public async Task<CleanClientResponseDto> GetByIdAsync(string id)
    {
        var client = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Клієнта з ID {id} не знайдено.");
        return _mapper.Map<CleanClientResponseDto>(client);
    }

    public async Task<CleanClientResponseDto> CreateAsync(CreateCleanClientDto dto)
    {
        var entity = _mapper.Map<CleanClient>(dto);
        await _repository.CreateAsync(entity);
        return _mapper.Map<CleanClientResponseDto>(entity);
    }

    public async Task<CleanClientResponseDto> UpdateAsync(string id, UpdateCleanClientDto dto)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Клієнта з ID {id} не знайдено.");

        var updated = _mapper.Map<CleanClient>(dto);
        updated.Id = existing.Id;
        await _repository.UpdateAsync(id, updated);
        return _mapper.Map<CleanClientResponseDto>(updated);
    }

    public async Task DeleteAsync(string id)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Клієнта з ID {id} не знайдено.");
        await _repository.DeleteAsync(id);
    }
}