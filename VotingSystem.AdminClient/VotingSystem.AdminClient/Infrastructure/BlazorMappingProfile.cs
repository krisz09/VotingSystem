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
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.PollOptions));
            CreateMap<PollOptionDto, PollOptionViewModel>();
            CreateMap<VoterDto, VoterViewModel>();


        }
    }
}
