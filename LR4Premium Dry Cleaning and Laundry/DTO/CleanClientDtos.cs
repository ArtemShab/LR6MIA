using System.ComponentModel.DataAnnotations;

namespace DryCleaningApi.DTOs;

public class CreateCleanClientDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool IsVip { get; set; } = false;
}

public class UpdateCleanClientDto : CreateCleanClientDto { }

public class CleanClientResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsVip { get; set; }
}