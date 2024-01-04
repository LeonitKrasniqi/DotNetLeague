using DotNetLeague.API.Models.Entities;

namespace DotNetLeague.API.Repositories
{
    public interface ITeamsRepository
    {
        Task<List<Team>> GetTeamsAsync();
        Task<Team>GetByIdAsync(Guid id);

        Task<Team> CreateAsync(Team team);
    }
}
