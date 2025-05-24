namespace VotingSystem.AdminClient.ViewModels
{
    public class PollViewModel
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }   
        public List<PollOptionViewModel> Options { get; set; } = new();
        public List<VoterViewModel> Voters { get; set; } = new();
        public bool HasVotes { get; set; }

        public int MinVotes { get; set; }
        public int MaxVotes { get; set; }
        // Optional UI helpers
        public string FormattedStart => StartDate.ToString("yyyy.MM.dd HH:mm");
        public string FormattedEnd => EndDate.ToString("yyyy.MM.dd HH:mm");

    }
} 