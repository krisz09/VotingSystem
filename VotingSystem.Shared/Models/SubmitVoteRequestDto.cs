using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class SubmitVoteRequestDto
    {
        public int PollOptionId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
