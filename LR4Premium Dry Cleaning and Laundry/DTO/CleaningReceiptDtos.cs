using System.ComponentModel.DataAnnotations;
using DryCleaningApi.Models.Enums;

namespace DryCleaningApi.DTOs;

public class AcceptOrderRequestDto
{
    [Required]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    public List<CreateGarmentItemDto> Items { get; set; } = new();
}

public class ProcessCleaningRequestDto
{
    public bool IsExpress24h { get; set; }
    public bool PassedQualityControl { get; set; }
}

public class IssueOrderRequestDto
{
    [Required]
    public string ReceiptNumberOrPhone { get; set; } = string.Empty;

    public bool PayNow { get; set; } = true;
    public bool IsDamagedByStaff { get; set; } = false;

    [Range(0, 100)]
    public int CompensationPercentage { get; set; } = 100;
}

public class CleaningReceiptResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string ReceiptNumber { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public List<string> GarmentItemIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime EstimatedReadyDate { get; set; }
    public DateTime? ClosedAt { get; set; }
    public ReceiptStatus Status { get; set; }
    public QueuePriority Priority { get; set; }
    public bool IsExpress24h { get; set; }
    public bool PassedQualityControl { get; set; }
    public bool IsPackaged { get; set; }
    public bool SmsNotificationSent { get; set; }
    public decimal BaseCost { get; set; }
    public decimal FinalCost { get; set; }
    public bool IsPaid { get; set; }
    public bool IsDamagedByStaff { get; set; }
    public string? WarrantyActNumber { get; set; }
    public decimal InsurancePayoutAmount { get; set; }
}