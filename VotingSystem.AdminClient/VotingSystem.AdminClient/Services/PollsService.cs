using AutoMapper;
using VotingSystem.AdminClient.Infrastructure;
using VotingSystem.AdminClient.Services;
using VotingSystem.AdminClient.ViewModels;
using VotingSystem.Shared.Models;

namespace VotingSystem.AdminClient.Services
{

    public class PollsService : IPollsService
    {
        private readonly IHttpRequestUtility _httpRequestUtility;
        private readonly IMapper _mapper;

        public PollsService(IHttpRequestUtility httpRequestUtility, IMapper mapper)
        {
            _httpRequestUtility = httpRequestUtility;
            _mapper = mapper;
        }

        public async Task<List<PollViewModel>> GetPollsCreatedByUserAsync()
        {
            var result = await _httpRequestUtility.ExecuteGetHttpRequestAsync<List<PollResponseDto>>("api/votes/mypolls");
            return _mapper.Map<List<PollViewModel>>(result.Response);
        }

        public async Task<bool> CreatePollAsync(CreatePollViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Question))
            {
                throw new ArgumentException("Question cannot be null or empty.", nameof(vm.Question));
            }

            if (!vm.StartDate.HasValue || !vm.EndDate.HasValue)
            {
                throw new ArgumentException("StartDate and EndDate must have valid values.");
            }

            var dto = new CreatePollRequestDto
            {
                Question = vm.Question,
                StartDate = DateTime.SpecifyKind(vm.StartDate.Value, DateTimeKind.Local),
                EndDate = DateTime.SpecifyKind(vm.EndDate.Value, DateTimeKind.Local),
                MinVotes = vm.minVotes,
                MaxVotes = vm.maxVotes,
                Options = vm.Options.Where(opt => !string.IsNullOrWhiteSpace(opt)).ToList()
            };

            await _httpRequestUtility.ExecutePostHttpRequestAsync<CreatePollRequestDto, PollResponseDto>("api/votes/create", dto);
            return true;
        }

        public async Task<bool> UpdatePollAsync(PollViewModel vm)
        {
            var dto = _mapper.Map<UpdatePollRequestDto>(vm); // vagy PollViewModel-t küldesz, ha az elég

            var result = await _httpRequestUtility.ExecutePutHttpRequestAsync<UpdatePollRequestDto ,PollResponseDto>(
                $"api/votes/{vm.Id}", dto);

            return true;
        }

    }
}
