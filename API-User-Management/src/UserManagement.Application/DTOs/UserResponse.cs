namespace UserManagement.Application.DTOs;

public sealed record UserResponse(
    Guid Id,
    string Name,
    int Age,
    string City,
    string State,
    string Pincode,
    DateTime CreatedAtUtc);
