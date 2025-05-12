using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Services
{
    public class PollsService : IPollsService
    {
        private readonly VotingSystemDbContext _context;

        public PollsService(VotingSystemDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Poll>> GetAllPollsAsync()
        {
            return await _context.Polls
                .Include(p => p.PollOptions)
                .ToListAsync();
        }

        public async Task<(List<Poll> Polls, List<int> VotedPollIds)> GetActivePollsWithVotesAsync(string userId)
        {
            var polls = await _context.Polls
                .Include(p => p.PollOptions)
                .Where(p => p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now)
                .ToListAsync();

            var votedPollIds = await _context.Votes
                .Where(v => v.UserId == userId)
                .Select(v => v.PollOption.PollId)
                .Distinct()
                .ToListAsync();

            return (polls, votedPollIds);
        }

        public async Task<Poll?> GetPollByIdAsync(int id)
        {
            return await _context.Polls
                .Include(p => p.PollOptions)
                    .ThenInclude(po => po.Votes)
                .ThenInclude(v => v.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<IReadOnlyCollection<Poll>> GetClosedPollsAsync(string? questionText, DateTime? startDate, DateTime? endDate)
        {
            var closedPollsQuery = _context.Polls
                .Where(p => p.EndDate < DateTime.Now); // Polls that have ended

            if (!string.IsNullOrEmpty(questionText))
            {
                closedPollsQuery = closedPollsQuery.Where(p => p.Question.Contains(questionText));
            }

            if (startDate.HasValue)
            {
                closedPollsQuery = closedPollsQuery.Where(p => p.EndDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                closedPollsQuery = closedPollsQuery.Where(p => p.EndDate <= endDate.Value);
            }

            return await closedPollsQuery.ToListAsync();
        }

        public async Task<IReadOnlyCollection<Poll>> GetPollsCreatedByUser(string userId)
        {
            var polls = await _context.Polls
                .Include(p => p.PollOptions)
                    .ThenInclude(o => o.Votes)
                        .ThenInclude(v => v.User)
                .Where(p => p.CreatedByUserId == userId)
                .ToListAsync();

            return polls;
        }

        public async Task<bool> CreatePollAsync(Poll newPoll)
        {
            _context.Polls.Add(newPoll);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<List<int>> GetVotedPollIdsForUserAsync(string userId)
        {
            return await _context.Votes
                .Where(v => v.UserId == userId)
                .Select(v => v.PollOption.PollId)
                .Distinct()
                .ToListAsync();
        }



        public async Task<bool> SubmitVotesAsync(List<int> pollOptionIds, string userId)
        {
            if (pollOptionIds == null || !pollOptionIds.Any())
                return false;

            // Az első opció alapján lekérjük a szavazáshoz tartozó Poll-t (feltételezzük, hogy az összes opció ugyanahhoz tartozik)
            var firstOption = await _context.PollOptions
                .Include(po => po.Poll)
                .FirstOrDefaultAsync(po => po.Id == pollOptionIds[0]);

            if (firstOption == null || firstOption.Poll.EndDate < DateTime.Now)
                return false;

            var poll = firstOption.Poll;

            // Ellenőrzés: minden opció ehhez a Poll-hoz tartozik-e
            var validOptionIds = await _context.PollOptions
                .Where(po => po.PollId == poll.Id)
                .Select(po => po.Id)
                .ToListAsync();

            if (!pollOptionIds.All(id => validOptionIds.Contains(id)))
                return false;

            // Ellenőrzés: felhasználó már szavazott-e ebben a szavazásban
            var hasAlreadyVoted = await _context.Votes
                .AnyAsync(v => v.UserId == userId && validOptionIds.Contains(v.PollOptionId));

            if (hasAlreadyVoted)
                return false;

            // Min/Max validáció
            if (pollOptionIds.Count < poll.Minvotes || pollOptionIds.Count > poll.Maxvotes)
                return false;

            // Szavazatok létrehozása
            var votes = pollOptionIds.Select(optionId => new Vote
            {
                PollOptionId = optionId,
                UserId = userId,
                VotedAt = DateTime.Now
            });

            _context.Votes.AddRange(votes);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdatePollAsync(
            int pollId,
            string question,
            DateTime startDate,
            DateTime endDate,
            int minVotes,
            int maxVotes,
            List<string> options,
            string userId)
        {
            var poll = await _context.Polls
                .Include(p => p.PollOptions)
                    .ThenInclude(po => po.Votes) // 🔍 include votes via PollOption
                .FirstOrDefaultAsync(p => p.Id == pollId && p.CreatedByUserId == userId);

            if (poll == null)
                return false;

            // 🔄 check if any of the options have votes
            var hasVotes = poll.PollOptions.Any(po => po.Votes.Any());
            var now = DateTime.UtcNow;
            var canEditAll = poll.StartDate > now && !hasVotes;

            if (!canEditAll)
            {
                if (endDate <= poll.EndDate)
                    return false;

                poll.EndDate = endDate;
            }
            else
            {
                if (minVotes < 1 || maxVotes > options.Count || minVotes > maxVotes)
                    return false;

                poll.Question = question;
                poll.StartDate = startDate;
                poll.EndDate = endDate;
                poll.Minvotes = minVotes;
                poll.Maxvotes = maxVotes;

                _context.PollOptions.RemoveRange(poll.PollOptions);
                poll.PollOptions = options.Select(text => new PollOption
                {
                    OptionText = text,
                    PollId = poll.Id
                }).ToList();
            }

            await _context.SaveChangesAsync();
            return true;
        }


    }


}
