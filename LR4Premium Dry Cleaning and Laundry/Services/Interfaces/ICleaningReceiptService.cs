using DryCleaningApi.DTOs;

namespace DryCleaningApi.Services.Interfaces;

public interface ICleaningReceiptService
{
    Task<List<CleaningReceiptResponseDto>> GetAllAsync();
    Task<CleaningReceiptResponseDto> GetByIdAsync(string id);
    Task DeleteAsync(string id);

    Task<CleaningReceiptResponseDto> AcceptItemsAndCreateReceiptAsync(AcceptOrderRequestDto dto);
    Task<CleaningReceiptResponseDto> ProcessCleaningAndQualityControlAsync(string receiptId, ProcessCleaningRequestDto dto);
    Task<CleaningReceiptResponseDto> IssueOrderAndHandleCompensationAsync(IssueOrderRequestDto dto);
}