using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class PollOptionDto
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = null!;
    }
}
