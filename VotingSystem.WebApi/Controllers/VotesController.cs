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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // vagy ClaimTypes.NameIdentifier
            Console.WriteLine("userId from JWT: " + userId);
            if (userId == null) return Unauthorized();

            var activePolls = await _pollsService.GetActivePollsWithVotesAsync(userId);
            var pollsDto = _mapper.Map<List<PollResponseDto>>(activePolls);

            var votedPollIds = await _pollsService.GetVotedPollIdsForUserAsync(userId);
            foreach (var dto in pollsDto)
            {
                dto.HasVoted = votedPollIds.Contains(dto.Id);
            }

            return Ok(pollsDto);
        }

        [Authorize]
        [HttpGet("closed-polls")]
        public async Task<ActionResult> GetClosedPolls(
    [FromQuery] string? questionText,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate)
        {
            var closedPolls = await _pollsService.GetClosedPollsAsync(questionText, startDate, endDate);

            if (closedPolls == null || !closedPolls.Any())
            {
                var emptyList = new List<PollResponseDto>();
                return Ok(emptyList);
            }

            var closedPollsDto = _mapper.Map<List<PollResponseDto>>(closedPolls);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var votedPollIds = await _pollsService.GetVotedPollIdsForUserAsync(userId);
            foreach (var dto in closedPollsDto)
            {
                dto.HasVoted = votedPollIds.Contains(dto.Id);
            }

            return Ok(closedPollsDto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<PollResponseDto>>> GetAllPolls()
        {
            var polls = await _context.Polls
                .Include(p => p.PollOptions)
                .ToListAsync();

            var dto = _mapper.Map<List<PollResponseDto>>(polls);

            return Ok(dto);
        }



        [Authorize]
        [HttpGet("mypolls")]
        public async Task<ActionResult<List<PollResponseDto>>> GetMyPolls()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var polls = await _pollsService.GetPollsCreatedByUser(userId);

            var pollResponseDtos = _mapper.Map<List<PollResponseDto>>(polls);

            return Ok(pollResponseDtos);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePoll([FromBody] CreatePollRequestDto dto)
        {
            try
            {
                Console.WriteLine($"Question: {dto.Question}");
                Console.WriteLine($"StartDate: {dto.StartDate}");
                Console.WriteLine($"End: {dto.EndDate}");
                Console.WriteLine($"Options count: {dto.Options.Count}");

                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine("User ID from token: " + userId);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();


            // + Debug: ellenőrizzük, hogy az EF látja-e ezt a userId-t
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                Console.WriteLine("user in db:" + userExists);

            if (!userExists)
            {
                Console.WriteLine("User not found");
                return BadRequest("User not found in AspNetUsers table.");
            }


            if (dto.StartDate < DateTime.UtcNow || dto.EndDate < DateTime.UtcNow || (dto.EndDate - dto.StartDate).TotalMinutes < 15)
                return BadRequest("Invalid date range");

            var poll = _mapper.Map<Poll>(dto);
            poll.CreatedByUserId = userId;

                Console.WriteLine("Poll mapped success");

                await _pollsService.CreatePollAsync(poll);

                Console.WriteLine("Poll saved success");
            return Ok(); // Optionally return the created poll DTO if needed
            }
            catch(Exception ex)
            {
                Console.WriteLine("Hiba történt a createPoll során: " + ex.Message);
                return StatusCode(500, "server erro:" + ex.Message);
            }
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

        [HttpGet("{pollId}/results")]
        public async Task<IActionResult> GetPollResults(int pollId)
        {
            var poll = await _context.Polls
                .Include(p => p.PollOptions)
                    .ThenInclude(po => po.Votes)
                .FirstOrDefaultAsync(p => p.Id == pollId);

            if (poll == null)
            {
                return NotFound();
            }

            var resultDto = new PollResultDto
            {
                Id = poll.Id,
                Question = poll.Question,
                Options = poll.PollOptions.Select(option => new PollOptionResultDto
                {
                    Id = option.Id,
                    OptionText = option.OptionText,
                    VoteCount = option.Votes.Count  // <--- mapping vote count
                }).ToList()
            };

            return Ok(resultDto);
        }

        /* ----------- FOR TESTING
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPolls()
        {
            var polls = await _pollsService.GetAllPollsAsync();
            var mapped = _mapper.Map<List<PollResponseDto>>(polls);
            return Ok(mapped);
        }
        */

        [HttpPost("create-test-polls")]
        public async Task<IActionResult> CreateTestPolls()
        {
            var user = await _context.Users.FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("No users found in the database.");
            }

            if (!_context.Polls.Any())
            {
                var now = DateTime.UtcNow;

                var polls = new List<Poll>
        {
            // ✅ Active Poll
            new Poll
            {
                Question = "What is your favorite color?",
                StartDate = now.AddDays(-1),
                EndDate = now.AddDays(5),
                CreatedByUserId = user.Id,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "Red" },
                    new PollOption { OptionText = "Blue" },
                    new PollOption { OptionText = "Green" }
                }
            },
            new Poll
            {
                Question = "What is your programming language?",
                StartDate = now.AddDays(-1),
                EndDate = now.AddDays(5),
                CreatedByUserId = user.Id,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "C#" },
                    new PollOption { OptionText = "Java" },
                    new PollOption { OptionText = "C++" }
                }
            },

            new Poll
            {
                Question = "Which framework do you prefer?",
                StartDate = now.AddDays(-10),
                EndDate = now.AddDays(-1),
                CreatedByUserId = user.Id,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "React" },
                    new PollOption { OptionText = "Angular" },
                    new PollOption { OptionText = "Vue" }
                }
            },

            // ✅ Another Closed Poll
            new Poll
            {
                Question = "What is your favorite backend language?",
                StartDate = now.AddDays(-7),
                EndDate = now.AddDays(-2),
                CreatedByUserId = user.Id,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "C#" },
                    new PollOption { OptionText = "Java" },
                    new PollOption { OptionText = "Node.js" }
                }
            }
        };

                _context.Polls.AddRange(polls);
                await _context.SaveChangesAsync();

                return Ok("Test polls (active and closed) created.");
            }

            return Ok("Polls already exist — no new test polls created.");
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
