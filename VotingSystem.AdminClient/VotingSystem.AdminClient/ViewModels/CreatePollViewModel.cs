using System.ComponentModel.DataAnnotations;

public class CreatePollViewModel
{
    [Required]
    public string? Question { get; set; }

    [Required]
    public DateTime? StartDate { get; set; }

    [Required]
    public DateTime? EndDate { get; set; }

    [MinLength(2, ErrorMessage = "Legalább 2 opció szükséges.")]
    public List<string> Options { get; set; } = new();
}
    