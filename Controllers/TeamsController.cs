using AutoMapper;
using DotNetLeague.API.CustomActionFilters;
using DotNetLeague.API.Data;
using DotNetLeague.API.Models.DTOs;
using DotNetLeague.API.Models.Entities;
using DotNetLeague.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetLeague.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly DotNetLeagueDbContext dbContext;
        private readonly ITeamsRepository teamsRepository;
        private readonly IMapper mapper;

        public TeamsController(DotNetLeagueDbContext dbContext, ITeamsRepository teamsRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.teamsRepository = teamsRepository;
            this.mapper = mapper;   
        }


        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAll()
        {

            var teamsEntity = await teamsRepository.GetTeamsAsync();
            if (teamsEntity == null || teamsEntity.Count == 0)
            {
                return NotFound("There are no teams in the database.");
            }


            return Ok(mapper.Map<List<TeamDto>>(teamsEntity));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetById(Guid id) {

            var team = await teamsRepository.GetByIdAsync(id);
          

            return Ok(mapper.Map<TeamDto>(team));
        
        }


        [HttpPost]
        [ValidateModel]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> Create([FromBody]AddTeamDto addTeamDto)
        {

            if (await dbContext.Teams.CountAsync() >= 16)
            {
                return BadRequest("Maximum number of teams reached. Cannot create more teams.");
            }
            var teamEntitiesModel = mapper.Map<Team>(addTeamDto);

            teamEntitiesModel = await teamsRepository.CreateAsync(teamEntitiesModel);


            var teamDto = mapper.Map<TeamDto>(teamEntitiesModel);   

            return CreatedAtAction(nameof(GetById), new {id = teamDto.TeamId},teamDto);
        }


    }
}
