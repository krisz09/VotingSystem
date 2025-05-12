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
        [HttpGet("active")]
        public async Task<IActionResult> GetActivePolls()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var (polls, votedPollIds) = await _pollsService.GetActivePollsWithVotesAsync(userId);

            var result = polls.Select(p => new PollResponseDto
            {
                Id = p.Id,
                Question = p.Question,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                MinVotes = p.Minvotes,
                MaxVotes = p.Maxvotes,
                HasVoted = votedPollIds.Contains(p.Id),
                PollOptions = p.PollOptions.Select(opt => new PollOptionDto
                {
                    Id = opt.Id,
                    OptionText = opt.OptionText
                }).ToList()
            }).ToList();

            return Ok(result);
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

                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                Console.WriteLine("user in db:" + userExists);

                if (!userExists)
                    return BadRequest("User not found in AspNetUsers table.");

                if (dto.StartDate < DateTime.UtcNow || dto.EndDate < DateTime.UtcNow || (dto.EndDate - dto.StartDate).TotalMinutes < 15)
                    return BadRequest("Invalid date range");

                var poll = _mapper.Map<Poll>(dto);
                poll.CreatedByUserId = userId;

                Console.WriteLine("Poll mapped success");

                await _pollsService.CreatePollAsync(poll);

                Console.WriteLine("Poll saved success");

                var responseDto = _mapper.Map<PollResponseDto>(poll);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba történt a createPoll során: " + ex.Message);
                return StatusCode(500, "server error: " + ex.Message);
            }
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePoll(int id, [FromBody] UpdatePollRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _pollsService.UpdatePollAsync(
                id,                            // comes from URL
                dto.Question,
                dto.StartDate,
                dto.EndDate,
                dto.MinVotes,
                dto.MaxVotes,
                dto.Options,
                userId                         // from authenticated user
            );

            if (!success)
                return BadRequest("Poll could not be updated.");



            var updatedPoll = await _pollsService.GetPollByIdAsync(id);

            if (updatedPoll == null)
                return NotFound();
            var dtoResponse = _mapper.Map<PollResponseDto>(updatedPoll);

            return Ok(dtoResponse);
        }



        [HttpPost("submit-vote")]
        public async Task<IActionResult> SubmitVote([FromBody] SubmitVoteRequestDto request)
        {
            // Alap validáció
            if (request.PollOptionIds == null || request.PollOptionIds.Count == 0 || string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest("Invalid vote submission.");
            }

            // Szolgáltatás meghívása
            var result = await _pollsService.SubmitVotesAsync(request.PollOptionIds, request.UserId);

            if (result)
            {
                return Ok("Vote submitted successfully.");
            }

            return BadRequest("Failed to submit vote. Either already voted, poll ended, or invalid option selection.");
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
                Options = poll.PollOptions.Select(option => new PollOptionDto
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
