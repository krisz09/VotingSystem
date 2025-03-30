using AutoMapper;
using VotingSystem.DataAccess;
using VotingSystem.Shared.Models;

namespace VotingSystem.WebApi.Infrastructure
{
    public class Mappingprofile : Profile
    {
        public Mappingprofile()
        {
            CreateMap<LoginDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<Poll, PollResponseDto>()
                .ForMember(dest => dest.PollOptions, opt => opt.MapFrom(src => src.PollOptions));
            CreateMap<PollOption, PollOptionDto>();
        }
    }
}
