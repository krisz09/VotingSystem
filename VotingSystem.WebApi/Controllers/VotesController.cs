using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VotingSystem.DataAccess;
using VotingSystem.DataAccess.Services;
using VotingSystem.Shared.Models;

namespace VotingSystem.WebApi.Controllers
{
    [Route("api/votes")]
    [ApiController]
    public class VotesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPollsService _pollsService;
        private readonly VotingSystemDbContext _context;
        public VotesController(IPollsService pollsService, IMapper mapper, VotingSystemDbContext context)
        {
            _pollsService = pollsService;
            _mapper = mapper;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetActivePolls()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub); // vagy ClaimTypes.NameIdentifier
            if (userId == null) return Unauthorized();

            var activePolls = await _pollsService.GetActivePollsWithVotesAsync(userId);
            var pollsDto = _mapper.Map<List<PollResponseDto>>(activePolls);

            // állítsuk be a HasVoted mezőt DTO-n belül is
            var votedPollIds = await _pollsService.GetVotedPollIdsForUserAsync(userId);
            foreach (var dto in pollsDto)
            {
                dto.HasVoted = votedPollIds.Contains(dto.Id);
            }

            return Ok(pollsDto);
        }


        [HttpPost("submit-vote")]
        public async Task<IActionResult> SubmitVote([FromBody] SubmitVoteRequestDto request)
        {
            if (request.PollOptionId <= 0 || string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest("Invalid vote submission.");
            }

            var result = await _pollsService.SubmitVoteAsync(request.PollOptionId, request.UserId);
            if (result)
            {
                return Ok("Vote submitted successfully.");
            }

            return BadRequest("Failed to submit vote.");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPolls()
        {
            var polls = await _pollsService.GetAllPollsAsync();
            var mapped = _mapper.Map<List<PollResponseDto>>(polls);
            return Ok(mapped);
        }

        [HttpPost("create-test-polls")]
        public async Task<IActionResult> CreateTestPolls()
        {
            var user = await _context.Users.FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("Nincs felhasználó az adatbázisban.");
            }

            if (!_context.Polls.Any())
            {
                var polls = new List<Poll>
        {
            new Poll
            {
                Question = "Mi a kedvenc színed?",
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
                CreatedByUserId = user.Id,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "Piros" },
                    new PollOption { OptionText = "Kék" },
                    new PollOption { OptionText = "Zöld" }
                }
            },
            new Poll
            {
                Question = "Melyik nyelvet szereted?",
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
                CreatedByUserId = user.Id,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "C#" },
                    new PollOption { OptionText = "Java" },
                    new PollOption { OptionText = "Python" }
                }
            }
                };

                _context.Polls.AddRange(polls);
                await _context.SaveChangesAsync();

                return Ok("Teszt szavazások létrehozva.");
            }

            return Ok("Már léteznek szavazások, nem hoztam létre újakat.");
        }

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllPolls()
        {
            var votes = await _context.Votes.ToListAsync();
            var pollOptions = await _context.PollOptions.ToListAsync();
            var polls = await _context.Polls.ToListAsync();

            _context.Votes.RemoveRange(votes);
            _context.PollOptions.RemoveRange(pollOptions);
            _context.Polls.RemoveRange(polls);

            await _context.SaveChangesAsync();

            return Ok("Minden szavazás, opció és szavazat törölve.");
        }


    }
}
