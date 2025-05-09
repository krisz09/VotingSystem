using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class UpdatePollRequestDto
    {

        public string Question { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<string> Options { get; set; } = new();

        public int MinVotes { get; set; }

        public int MaxVotes { get; set; }
    }
}
