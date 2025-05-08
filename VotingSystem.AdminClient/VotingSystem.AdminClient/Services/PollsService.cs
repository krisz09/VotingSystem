using AutoMapper;
using VotingSystem.AdminClient.Infrastructure;
using VotingSystem.AdminClient.Services;
using VotingSystem.AdminClient.ViewModels;
using VotingSystem.Shared.Models;

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
        var dto = new CreatePollRequestDto
        {
            Question = vm.Question,
            StartDate = vm.StartDate!.Value,
            EndDate = vm.EndDate!.Value,
            Options = vm.Options.Where(opt => !string.IsNullOrWhiteSpace(opt)).ToList()
        };

        try
        {
            await _httpRequestUtility.ExecutePostHttpRequestAsync<CreatePollRequestDto, PollResponseDto>("api/votes/create", dto);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Problemo");
            return false;
        }
    }




}
