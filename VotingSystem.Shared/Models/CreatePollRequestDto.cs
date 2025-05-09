using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Shared.Models
{
    public class CreatePollRequestDto
    {
        [Required(ErrorMessage = "Question is required")]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        public int MinVotes { get; set; }

        public int MaxVotes { get; set; }

        [MinLength(2, ErrorMessage = "At least two options are required")]
        public List<string> Options { get; set; } = new();
    }
}
