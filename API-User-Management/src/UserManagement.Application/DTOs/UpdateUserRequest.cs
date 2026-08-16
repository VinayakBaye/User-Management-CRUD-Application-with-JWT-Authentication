using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs;

public sealed class UpdateUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Range(0, 120)]
    public int Age { get; init; }

    [Required]
    public string City { get; init; } = string.Empty;

    [Required]
    public string State { get; init; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 4)]
    public string Pincode { get; init; } = string.Empty;
}