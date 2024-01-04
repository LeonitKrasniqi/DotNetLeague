using DotNetLeague.API.Data;
using DotNetLeague.API.Models.DTOs;
using DotNetLeague.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetLeague.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {

        private readonly ILeagueRepository leagueRepository;
        public LeagueController(ILeagueRepository leagueRepository)
        {
            this.leagueRepository = leagueRepository;
        }

        [HttpPost("generate-groups")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GenerateTeamsIntoGroup()
        {
                await leagueRepository.GenerateDrawAsync();

                return Ok("Teams have been generated into groups.");
            
        }



        [HttpGet("league")]
        [Authorize(Roles ="Admin, User")]
        public async Task<IActionResult> GetGeneratedLeague()
        {
            try
            {
                var generatedLeague = await leagueRepository.GetGeneratedLeagueAsync();
                return Ok(generatedLeague);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while getting the generated league.");
            }
        }

        [HttpPost("generate-group-matches")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GenerateGroupMatches()
        {
            try
            {
                var generatedMatches = await leagueRepository.GenerateGroupMatchesAsync();
                return Ok(generatedMatches);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("best-teams-per-group")]
        [Authorize(Roles ="Admin, User")]
        public async Task<IActionResult> GetBestTeamsPerGroup()
        {
            try
            {
                var bestTeams = await leagueRepository.GetBestTeamsPerGroupAsync();
                return Ok(bestTeams);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("generate-semifinal-matches")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GenerateFinalMatches()
        {
            try
            {
                var finalMatches = await leagueRepository.GenerateSemiFinalMatchesAsync();
                return Ok(finalMatches);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("best-teams-from-semifinals")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetBestTeamsFromSemifinals()
        {
            try
            {
                var bestTeams = await leagueRepository.GetBestTeamsFromSemifinalsAsync();

                if (bestTeams == null || bestTeams.Count == 0)
                {
                    return NotFound("No best teams found from semifinals.");
                }

                return Ok(bestTeams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpPost("generate-final-match")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GenerateFinalMatch()
        {
            try
            {
                var finalMatch = await leagueRepository.GenerateFinalAsync();
                return Ok(finalMatch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("winner")]
        [Authorize(Roles ="Admin, User")]
        public async Task<IActionResult> GetWinner()
        {
            try
            {
                string winnerMessage = await leagueRepository.GetWinnerAsync();
                return Ok(winnerMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


    }
}
