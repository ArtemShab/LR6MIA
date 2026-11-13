using DryCleaningApi.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace DryCleaningApi.Models;
public class GarmentItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string ClientId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [BsonRepresentation(BsonType.String)]
    public GarmentCategory Category { get; set; }
    public string FabricType { get; set; } = string.Empty;
    [BsonRepresentation(BsonType.String)]
    public CleaningType CleaningType { get; set; }
    public int WearPercentage { get; set; }
    public bool ComplexStains { get; set; }
    public string DefectDescription { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal DeclaredValue { get; set; }
    public decimal BaseCleaningPrice { get; set; }
    public bool IsOnPickupShelf { get; set; } = false;
}