using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DryCleaningApi.Models;

public class CleanClient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsVip { get; set; } = false;
}