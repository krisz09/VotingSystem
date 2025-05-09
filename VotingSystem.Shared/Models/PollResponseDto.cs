using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class PollResponseDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedByUserId { get; set; } = null!;
        public int MinVotes { get; set; }
        public int MaxVotes { get; set; }

        public List<PollOptionDto> PollOptions { get; set; } = new List<PollOptionDto>();
        public List<VoterDto> Voters { get; set; } = new();

        public bool HasVoted { get; set; }
    }
}
