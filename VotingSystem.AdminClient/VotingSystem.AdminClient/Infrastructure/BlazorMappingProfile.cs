using AutoMapper;
using VotingSystem.AdminClient.ViewModels;
using VotingSystem.Shared.Models;

namespace VotingSystem.AdminClient.Infrastructure
{
    public class BlazorMappingProfile : Profile
    {
        public BlazorMappingProfile()
        {
            CreateMap<LoginViewModel, LoginRequestDto>(MemberList.Source);
            CreateMap<PollResponseDto, PollViewModel>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.PollOptions))
                .ForMember(dest => dest.HasVotes,
                       opt => opt.MapFrom(src => src.PollOptions.Any(po => po.VoteCount > 0)));

            CreateMap<PollOptionDto, PollOptionViewModel>();
            CreateMap<VoterDto, VoterViewModel>();
            CreateMap<PollViewModel, UpdatePollRequestDto>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src =>
                    src.Options.Select(o => o.OptionText).ToList()));



        }
    }
}
