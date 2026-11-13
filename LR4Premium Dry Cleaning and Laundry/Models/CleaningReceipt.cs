using DryCleaningApi.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DryCleaningApi.Models;

public class CleaningReceipt
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string ReceiptNumber { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string ClientId { get; set; } = string.Empty;

    public string ClientPhone { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> GarmentItemIds { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime EstimatedReadyDate { get; set; }
    public DateTime? ClosedAt { get; set; }

    [BsonRepresentation(BsonType.String)]
    public ReceiptStatus Status { get; set; } = ReceiptStatus.Accepted;

    [BsonRepresentation(BsonType.String)]
    public QueuePriority Priority { get; set; } = QueuePriority.Standard;

    public bool IsExpress24h { get; set; } = false;
    public bool PassedQualityControl { get; set; } = false;
    public bool IsPackaged { get; set; } = false;
    public bool SmsNotificationSent { get; set; } = false;

    public decimal BaseCost { get; set; }
    public decimal FinalCost { get; set; }
    public bool IsPaid { get; set; } = false;
    public bool IsDamagedByStaff { get; set; } = false;
    public string? WarrantyActNumber { get; set; }
    public decimal InsurancePayoutAmount { get; set; } = 0m;
}