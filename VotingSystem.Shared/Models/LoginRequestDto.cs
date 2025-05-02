using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Shared.Models;

/// <summary>
/// LoginRequestDTO
/// </summary>
public record LoginRequestDto
{
    /// <summary>
    /// Email of the user
    /// </summary>
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public required string Email { get; init; }

    /// <summary>
    /// Password of the user
    /// </summary>
    public required string Password { get; init; }
}
