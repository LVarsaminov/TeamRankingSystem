using AutoMapper;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Models;

namespace TeamRanking.Core.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Match, MatchDto>()
                .ForMember(dest => dest.Team1Name, opt => opt.MapFrom(src => src.Team1.Name))
                .ForMember(dest => dest.Team2Name, opt => opt.MapFrom(src => src.Team2.Name));

            CreateMap<CreateMatchDto, Match>()
                .ForPath(dest => dest.Team1.Name, opt => opt.MapFrom(src => src.Team1Name))
                .ForPath(dest => dest.Team2.Name, opt => opt.MapFrom(src => src.Team2Name))
                .ForMember(dest => dest.Team1Score, opt => opt.MapFrom(src => src.Team1Score))
                .ForMember(dest => dest.Team2Score, opt => opt.MapFrom(src => src.Team2Score))
                .ForMember(dest => dest.MatchDate, opt => opt.MapFrom(src => src.MatchDate));

            CreateMap<UpdateMatchDto, Match>()
                .ForMember(dest => dest.Team1Score, opt => opt.MapFrom(src => src.Team1Score))
                .ForMember(dest => dest.Team2Score, opt => opt.MapFrom(src => src.Team2Score))
                .ForMember(dest => dest.MatchDate, opt => opt.MapFrom(src => src.MatchDate))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Team, TeamDto>();
            CreateMap<CreateTeamDto, Team>();
            CreateMap<UpdateTeamDto, Team>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
