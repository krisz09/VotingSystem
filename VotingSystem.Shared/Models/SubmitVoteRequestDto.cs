﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class SubmitVoteRequestDto
    {
        public List<int> PollOptionIds { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
    }
}
