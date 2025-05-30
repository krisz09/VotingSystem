﻿using AutoMapper;
using VotingSystem.DataAccess;
using VotingSystem.Shared.Models;

namespace VotingSystem.WebApi.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map LoginDto to User
            CreateMap<UserRequestDto, User>(MemberList.Source)
            .ForSourceMember(src => src.Password, opt => opt.DoNotValidate())
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName));

            // Map RegisterDto to User
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // Map Poll to PollResponseDto
            CreateMap<Poll, PollResponseDto>()
                .ForMember(dest => dest.PollOptions, opt => opt.MapFrom(src => src.PollOptions))  // Map PollOptions
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))                       // Map Id (you can remove this if it's already mapped automatically)
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))           // Map Question
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))           // Map EndDate
                .ForMember(dest => dest.Voters, opt => opt.MapFrom(
                    src => src.PollOptions
                    .SelectMany(o => o.Votes)
                    .Select(v => v.User)
                    .Distinct()
                    ));

            // Map PollOption to PollOptionDto
            CreateMap<PollOption, PollOptionDto>()
                .ForMember(dest => dest.VoteCount, opt => opt.MapFrom(src => src.Votes.Count));



            CreateMap<CreatePollRequestDto, Poll>()
                .ForMember(dest => dest.PollOptions, opt => opt.MapFrom(src =>
                    src.Options.Select(opt => new PollOption
                    {
                        OptionText = opt
                    }).ToList()));


            CreateMap<User, VoterDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        }

    }
}
