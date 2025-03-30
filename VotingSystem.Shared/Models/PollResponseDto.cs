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
        public List<PollOptionDto> PollOptions { get; set; } = new List<PollOptionDto>();
    }
}
