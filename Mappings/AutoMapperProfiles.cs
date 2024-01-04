using AutoMapper;
using DotNetLeague.API.Models.DTOs;
using DotNetLeague.API.Models.Entities;

namespace DotNetLeague.API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Team, TeamDto>().ReverseMap();
            CreateMap<AddTeamDto,Team>().ReverseMap();
        }


    }
}
