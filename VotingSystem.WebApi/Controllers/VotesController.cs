using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.DataAccess.Services;
using VotingSystem.Shared.Models;

namespace VotingSystem.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class VotesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPollsService _pollsService;

        public VotesController(IPollsService pollsService, IMapper mapper)
        {
            _pollsService = pollsService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivePolls()
        {
            var activePolls = await _pollsService.GetActivePollsAsync();
            var pollsDto = _mapper.Map<List<PollResponseDto>>(activePolls);
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
    }
}
