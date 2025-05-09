using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
    }
}
