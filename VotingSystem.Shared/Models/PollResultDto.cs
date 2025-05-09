using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class PollResultDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = null!;
        public List<PollOptionDto> Options { get; set; } = new();
    }
}
