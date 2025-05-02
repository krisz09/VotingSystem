using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Shared.Models;

/// <summary>
/// UserRequestDTO
/// </summary>
public record UserRequestDto
{
    /// <summary>
    /// Name of the person making the reservation
    /// </summary>
    [StringLength(255, ErrorMessage = "Name is too long")]
    public required string Name { get; init; }


    /// <summary>
    /// Email of the person making the reservation
    /// </summary>
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public required string Email { get; init; }
    
    /// <summary>
    /// Password of the person making the reservation
    /// </summary>
    public required string Password { get; init; }

}
