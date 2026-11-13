using System.ComponentModel.DataAnnotations;
using DryCleaningApi.Models.Enums;

namespace DryCleaningApi.DTOs;

public class CreateGarmentItemDto
{
    [Required]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public GarmentCategory Category { get; set; }
    public string FabricType { get; set; } = string.Empty;
    public CleaningType CleaningType { get; set; }

    public int WearPercentage { get; set; }

    public bool ComplexStains { get; set; }
    public string DefectDescription { get; set; } = string.Empty;
    public decimal DeclaredValue { get; set; }
    public decimal BaseCleaningPrice { get; set; }
}

public class UpdateGarmentItemDto : CreateGarmentItemDto
{
    public bool IsOnPickupShelf { get; set; }
}

public class GarmentItemResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public GarmentCategory Category { get; set; }
    public string FabricType { get; set; } = string.Empty;
    public CleaningType CleaningType { get; set; }
    public int WearPercentage { get; set; }
    public bool ComplexStains { get; set; }
    public string DefectDescription { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal DeclaredValue { get; set; }
    public decimal BaseCleaningPrice { get; set; }
    public bool IsOnPickupShelf { get; set; }
}