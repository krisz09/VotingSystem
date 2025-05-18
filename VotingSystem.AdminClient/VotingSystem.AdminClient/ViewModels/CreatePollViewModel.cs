using System.ComponentModel.DataAnnotations;

namespace VotingSystem.AdminClient.ViewModels
{
    public class CreatePollViewModel
    {
        [Required]
        public string? Question { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

        [Required]
        public int minVotes { get; set; } = 1;

        [Required]
        public int maxVotes { get; set; }

        [MinLength(2, ErrorMessage = "Legalább 2 opció szükséges.")]
        public List<string> Options { get; set; } = new();
    }
}
    