using DotNetLeague.API.Models.DTOs;
using DotNetLeague.API.Models.Entities;

namespace DotNetLeague.API.Repositories
{
    public interface ILeagueRepository
    {
       Task<League> GenerateDrawAsync();
       Task<List<object>> GetGeneratedLeagueAsync();
        Task<List<MatchDto>> GenerateGroupMatchesAsync();
       Task<List<BestTeamsDto>> GetBestTeamsPerGroupAsync();
        Task<List<MatchDto>> GenerateSemiFinalMatchesAsync();
        Task<List<BestTeamsDto>> GetBestTeamsFromSemifinalsAsync();
        Task<MatchDto> GenerateFinalAsync();
        Task<string> GetWinnerAsync();
    }
}
