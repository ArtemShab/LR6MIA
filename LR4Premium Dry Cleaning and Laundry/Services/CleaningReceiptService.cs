using AutoMapper;
using DryCleaningApi.DTOs;
using DryCleaningApi.Models;
using DryCleaningApi.Models.Enums;
using DryCleaningApi.Repositories.Interfaces;
using DryCleaningApi.Services.Interfaces;

namespace DryCleaningApi.Services;

public class CleaningReceiptService : ICleaningReceiptService
{
    private readonly ICleaningReceiptRepository _receiptRepository;
    private readonly IGarmentItemRepository _garmentRepository;
    private readonly ICleanClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public CleaningReceiptService(
        ICleaningReceiptRepository receiptRepository,
        IGarmentItemRepository garmentRepository,
        ICleanClientRepository clientRepository,
        IMapper mapper)
    {
        _receiptRepository = receiptRepository;
        _garmentRepository = garmentRepository;
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<List<CleaningReceiptResponseDto>> GetAllAsync()
    {
        var receipts = await _receiptRepository.GetAllAsync();
        return _mapper.Map<List<CleaningReceiptResponseDto>>(receipts);
    }

    public async Task<CleaningReceiptResponseDto> GetByIdAsync(string id)
    {
        var receipt = await _receiptRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Квитанцію з ID {id} не знайдено.");
        return _mapper.Map<CleaningReceiptResponseDto>(receipt);
    }

    public async Task DeleteAsync(string id)
    {
        var existing = await _receiptRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Квитанцію з ID {id} не знайдено.");
        await _receiptRepository.DeleteAsync(id);
    }

    // =========================================================================
    // БІЗНЕС-ЗАДАЧА 1: Прийом речей та формування квитанції із дефектовкою
    // =========================================================================
    public async Task<CleaningReceiptResponseDto> AcceptItemsAndCreateReceiptAsync(AcceptOrderRequestDto dto)
    {
        var client = await _clientRepository.GetByIdAsync(dto.ClientId)
            ?? throw new KeyNotFoundException($"Клієнта з ID {dto.ClientId} не знайдено.");

        if (dto.Items == null || dto.Items.Count == 0)
            throw new ArgumentException("Квитанція повинна містити щонайменше одну річ.");

        var garmentIds = new List<string>();
        decimal baseCost = 0m;

        foreach (var itemDto in dto.Items)
        {
            // Правило 1: оцінюється відсоток зносу (від 10% до 50%)
            if (itemDto.WearPercentage < 10 || itemDto.WearPercentage > 50)
                throw new ArgumentException($"Річ '{itemDto.Name}' має некоректний відсоток зносу ({itemDto.WearPercentage}%). Допустимий діапазон: від 10% до 50%.");

            if (itemDto.BaseCleaningPrice <= 0)
                throw new ArgumentException($"Базова ціна чистки для '{itemDto.Name}' має бути більшою за 0.");

            // Правило 2: якщо присутні складні плями (ComplexStains), має бути опис дефекту
            if (itemDto.ComplexStains && string.IsNullOrWhiteSpace(itemDto.DefectDescription))
                throw new ArgumentException($"Для речі '{itemDto.Name}' із позначкою ComplexStains обов'язково вкажіть опис дефектів/фурнітури.");

            var garment = _mapper.Map<GarmentItem>(itemDto);
            garment.ClientId = client.Id!;

            // Правило 3: кожній речі присвоюється індивідуальний водостійкий штрихкод (RFID / Barcode)
            garment.Barcode = $"RFID-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            garment.IsOnPickupShelf = false;

            await _garmentRepository.CreateAsync(garment);
            garmentIds.Add(garment.Id!);
            baseCost += garment.BaseCleaningPrice;
        }

        // Правило 4: статус Accepted із розрахунковою датою готовності (стандартно 3 робочі дні)
        var estimatedReady = AddBusinessDays(DateTime.UtcNow, 3);

        var receipt = new CleaningReceipt
        {
            ReceiptNumber = $"REC-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
            ClientId = client.Id!,
            ClientPhone = client.PhoneNumber,
            GarmentItemIds = garmentIds,
            CreatedAt = DateTime.UtcNow,
            EstimatedReadyDate = estimatedReady,
            Status = ReceiptStatus.Accepted,
            Priority = QueuePriority.Standard,
            BaseCost = baseCost,
            FinalCost = baseCost
        };

        await _receiptRepository.CreateAsync(receipt);
        return _mapper.Map<CleaningReceiptResponseDto>(receipt);
    }

    public async Task<CleaningReceiptResponseDto> ProcessCleaningAndQualityControlAsync(string receiptId, ProcessCleaningRequestDto dto)
    {
        var receipt = await _receiptRepository.GetByIdAsync(receiptId)
            ?? throw new KeyNotFoundException($"Квитанцію з ID {receiptId} не знайдено.");

        if (receipt.Status == ReceiptStatus.Closed)
            throw new InvalidOperationException("Неможливо обробити вже закриту квитанцію.");

        receipt.Status = ReceiptStatus.Processing;
        receipt.IsExpress24h = dto.IsExpress24h;

        if (dto.IsExpress24h)
        {
            receipt.FinalCost = Math.Round(receipt.BaseCost * 1.50m, 2);
            receipt.Priority = QueuePriority.HighExpress;
            receipt.EstimatedReadyDate = receipt.CreatedAt.AddHours(24);
        }
        else
        {
            receipt.FinalCost = receipt.BaseCost;
            receipt.Priority = QueuePriority.Standard;
        }

        receipt.PassedQualityControl = dto.PassedQualityControl;

        if (dto.PassedQualityControl)
        {
            receipt.IsPackaged = true;
            receipt.Status = ReceiptStatus.ReadyForPickup;
            receipt.SmsNotificationSent = true;

            var garments = await _garmentRepository.GetByIdsAsync(receipt.GarmentItemIds);
            foreach (var item in garments)
            {
                item.IsOnPickupShelf = true;
                await _garmentRepository.UpdateAsync(item.Id!, item);
            }
        }

        await _receiptRepository.UpdateAsync(receipt.Id!, receipt);
        return _mapper.Map<CleaningReceiptResponseDto>(receipt);
    }

    public async Task<CleaningReceiptResponseDto> IssueOrderAndHandleCompensationAsync(IssueOrderRequestDto dto)
    {
        var receipt = await _receiptRepository.GetByReceiptNumberAsync(dto.ReceiptNumberOrPhone);

        if (receipt == null)
        {
            var byPhone = await _receiptRepository.GetByClientPhoneAsync(dto.ReceiptNumberOrPhone);
            receipt = byPhone.FirstOrDefault(r => r.Status == ReceiptStatus.ReadyForPickup)
                      ?? byPhone.FirstOrDefault();
        }

        if (receipt == null)
            throw new KeyNotFoundException($"Замовлення за номером квитанції або телефоном '{dto.ReceiptNumberOrPhone}' не знайдено.");

        if (receipt.Status == ReceiptStatus.Closed)
            throw new InvalidOperationException("Це замовлення вже видано та закрито.");

        if (receipt.Status != ReceiptStatus.ReadyForPickup)
            throw new InvalidOperationException($"Замовлення ще не готове до видачі (поточний статус: {receipt.Status}).");

        if (!receipt.IsPaid && !dto.PayNow)
            throw new InvalidOperationException("Видача неможлива без повної оплати послуг.");

        receipt.IsPaid = true;

        var garments = await _garmentRepository.GetByIdsAsync(receipt.GarmentItemIds);

        if (dto.IsDamagedByStaff)
        {
            receipt.IsDamagedByStaff = true;
            receipt.WarrantyActNumber = $"ACT-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100, 999)}";

            decimal totalDeclaredValue = garments.Sum(g => g.DeclaredValue);
            decimal factor = Math.Clamp(dto.CompensationPercentage, 0, 100) / 100m;
            receipt.InsurancePayoutAmount = Math.Round(totalDeclaredValue * factor, 2);
        }

        foreach (var item in garments)
        {
            item.IsOnPickupShelf = false;
            await _garmentRepository.UpdateAsync(item.Id!, item);
        }

        receipt.Status = ReceiptStatus.Closed;
        receipt.ClosedAt = DateTime.UtcNow;

        await _receiptRepository.UpdateAsync(receipt.Id!, receipt);
        return _mapper.Map<CleaningReceiptResponseDto>(receipt);
    }

    private static DateTime AddBusinessDays(DateTime startDate, int businessDays)
    {
        var current = startDate;
        int added = 0;
        while (added < businessDays)
        {
            current = current.AddDays(1);
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
            {
                added++;
            }
        }
        return current;
    }
}