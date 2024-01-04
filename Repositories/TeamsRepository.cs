using DotNetLeague.API.Data;
using DotNetLeague.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetLeague.API.Repositories
{
    public class TeamsRepository : ITeamsRepository
    {
        private readonly DotNetLeagueDbContext dbContext;
        public TeamsRepository(DotNetLeagueDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Team> CreateAsync(Team team)
        {
            await dbContext.Teams.AddAsync(team);
            await dbContext.SaveChangesAsync();
            return team;
        }

        public async Task<Team> GetByIdAsync(Guid id)
        {
            return await dbContext.Teams.FirstOrDefaultAsync(x => x.TeamId == id); 
        }

        public async Task<List<Team>> GetTeamsAsync()
        {
            return await dbContext.Teams.ToListAsync();
        }
    }
}
