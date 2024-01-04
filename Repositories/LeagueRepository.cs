using DotNetLeague.API.Data;
using DotNetLeague.API.Models.DTOs;
using DotNetLeague.API.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DotNetLeague.API.Repositories
{
    public class LeagueRepository : ILeagueRepository
    {
        private readonly DotNetLeagueDbContext dbContext;
        public LeagueRepository(DotNetLeagueDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<League> GenerateDrawAsync()
        {

            bool hasExistingDraw = await dbContext.League.AnyAsync();


            if (hasExistingDraw)
            {
                throw new Exception("Cannot generate draw. There is already a draw in the database.");

            }


            List<Team> teams = await dbContext.Teams.ToListAsync();


            if(teams.Count != 16) {

                throw new Exception ("Cannot generate draw. There must be exactly 16 teams.");
            }


            Random random = new Random();
            List<Team> randomizedTeam = teams.OrderBy(t => random.Next()).ToList();

            int groupCount = 4;

            int teamsPerGroup = teams.Count / groupCount;

            for (int i = 0; i < groupCount; i++)
            {
                var groupTeams = randomizedTeam.Skip(i * teamsPerGroup).Take(teamsPerGroup);

                var group = new Group
                {
                    GroupId = Guid.NewGuid(),
                    GroupName = (char)('A' + i)
                };

                foreach (var team in groupTeams)
                {
                    dbContext.League.Add(new League
                    {
                        TeamId = team.TeamId,
                        Team = team,
                        GroupId = group.GroupId,
                        Group = group
                    });
                }
            }

            await dbContext.SaveChangesAsync();
           
            return new League();
        }

        public async Task<List<object>> GetGeneratedLeagueAsync()
        {

            var hasTeamsAndGroups = await dbContext.League.AnyAsync();

            if(!hasTeamsAndGroups)
            {
                return null;
            }

            var result = await dbContext.League
                .Include(l => l.Team)
                .Include(l => l.Group)
                .GroupBy(l => l.Group.GroupName)
                .Select(g => new
                {
                    GroupName = g.Key,
                    Teams = g.Select(l => l.Team.TeamName).ToList()
                })
                .ToListAsync();

            return result.Select(item => new
            {
                GroupName = item.GroupName,
                Teams = item.Teams
            }).Cast<object>().ToList();
        }


        public async Task<List<MatchDto>> GenerateGroupMatchesAsync()
        {
            var existingMatchesCount = await dbContext.Matches.CountAsync();

            if (existingMatchesCount  >= 24)
            {
                throw new Exception("You cannot generate games more than 3 times.");
            }

            var totalTeams = await dbContext.Teams.CountAsync();
            if (totalTeams != 16) {
                throw new Exception("Cannot generate games, there are not 16 teams");
            }

            var groups = await dbContext.Groups.ToListAsync();
            var matches = new List<MatchDto>();
            var random = new Random();

            foreach (var group in groups)
            {
                var teamsInGroup = await dbContext.League
                    .Include(l => l.Team)
                    .Where(l => l.GroupId == group.GroupId)
                    .Select(l => l.Team)
                    .ToListAsync();

                if (teamsInGroup.Count < 4)
                {
                    throw new Exception($"Cannot generate matches for group {group.GroupName}. There must be at least four teams.");
                }

                teamsInGroup = teamsInGroup.OrderBy(t => random.Next()).ToList();

                var match1 = GenerateMatch(group, teamsInGroup[0], teamsInGroup[1], random);
                matches.Add(match1);

                var match2 = GenerateMatch(group, teamsInGroup[2], teamsInGroup[3], random);
                matches.Add(match2);
              
            }

            return matches;
        }

        private MatchDto GenerateMatch(Group group, Team team1, Team team2, Random random)
        {
            var match = new Match
            {
                MatchId = Guid.NewGuid(),
                GroupId = group.GroupId,
                Team1Id = team1.TeamId,
                Team2Id = team2.TeamId,
                ScoreTeam1 = random.Next(0, 5),
                ScoreTeam2 = random.Next(0, 5),
                PointsTeam1 = 0,
                PointsTeam2 = 0
            };

            if (match.ScoreTeam1 > match.ScoreTeam2)
            {
                match.PointsTeam1 = 3;
            }
            else if (match.ScoreTeam1 < match.ScoreTeam2)
            {
                match.PointsTeam2 = 3;
            }
            else
            {
                match.PointsTeam1 = 1;
                match.PointsTeam2 = 1;
            }

            dbContext.Matches.Add(match);
            dbContext.SaveChanges();

            var matchDto = new MatchDto
            {
                GroupName = group.GroupName,
                Team1 = team1.TeamName,
                Team2 = team2.TeamName,
                ScoreTeam1 = match.ScoreTeam1,
                ScoreTeam2 = match.ScoreTeam2,
                PointsTeam1 = match.PointsTeam1,
                PointsTeam2 = match.PointsTeam2
            };

            return matchDto;
        }



        private int GetTeamPointsInGroup(Team team, Guid groupId)
        {
            var teamMatches = dbContext.Matches
                .Where(m => m.GroupId == groupId && (m.Team1Id == team.TeamId || m.Team2Id == team.TeamId))
                .ToList();

            var points = 0;

            foreach (var match in teamMatches)
            {
                if (match.Team1Id == team.TeamId)
                {
                    points += match.PointsTeam1;
                }
                else
                {
                    points += match.PointsTeam2;
                }
            }

            return points;
        }

        public async Task<List<BestTeamsDto>> GetBestTeamsPerGroupAsync()
        {
            var groups = await dbContext.Groups.ToListAsync();

            if (!groups.Any())
            {
                return null;
            }

            var bestTeams = new List<BestTeamsDto>();

            foreach (var group in groups)
            {
                var teamsInGroup = await dbContext.League
                    .Include(l => l.Team)
                    .Where(l => l.GroupId == group.GroupId)
                    .Select(l => l.Team)
                    .ToListAsync();

                if (teamsInGroup.Count < 4)
                {
                    throw new Exception($"There must be at least four teams in group {group.GroupName}.");
                }

                var bestTeamInGroup = teamsInGroup
                    .OrderByDescending(t => GetTeamPointsInGroup(t, group.GroupId))
                    .FirstOrDefault();

                if (bestTeamInGroup != null)
                {
                    var bestTeamDto = new BestTeamsDto
                    {
                        TeamId = bestTeamInGroup.TeamId,
                        TeamName = bestTeamInGroup.TeamName,
                        Points = GetTeamPointsInGroup(bestTeamInGroup, group.GroupId)
                        ,GroupId = group.GroupId    
                    };

                    bestTeams.Add(bestTeamDto);
                }
            }

            return bestTeams;
        }

        public async Task<List<MatchDto>> GenerateSemiFinalMatchesAsync()
        {

            var existingMatchesCount = await dbContext.Matches.CountAsync();

            
            if (existingMatchesCount != 24)
            {
                throw new Exception("Cannot generate semi-finals!");
            }

            var bestTeams = await GetBestTeamsPerGroupAsync();

            if (bestTeams.Count != 4)
            {
                throw new Exception("There must be exactly four best teams to generate semi-final matches.");
            }

            var matches = new List<MatchDto>();
            var random = new Random();

            var match1 = GenerateSemiFinalMatch(bestTeams[0], bestTeams[1], random);
            matches.Add(match1);

            var match2 = GenerateSemiFinalMatch(bestTeams[2], bestTeams[3], random);
            matches.Add(match2);

            return matches;
        }

        private MatchDto GenerateSemiFinalMatch(BestTeamsDto team1, BestTeamsDto team2, Random random)
        {
            try
            {
                var match = new Match
                {
                    MatchId = Guid.NewGuid(),
                    Team1Id = team1.TeamId,
                    Team2Id = team2.TeamId,
                    ScoreTeam1 = random.Next(0, 5),
                    ScoreTeam2 = random.Next(0, 5),
                    PointsTeam1 = 0,
                    PointsTeam2 = 0,
                    GroupId =team1.GroupId,
                };

                if (match.ScoreTeam1 > match.ScoreTeam2)
                {
                    match.PointsTeam1 = 3;
                }
                else if (match.ScoreTeam1 < match.ScoreTeam2)
                {
                    match.PointsTeam2 = 3;
                }
                else
                {
                    // Scores are equal; randomly assign 3 points to one of the teams
                    var randomWinner = random.Next(0, 2);

                    if (randomWinner == 0)
                    {
                        match.PointsTeam1 = 3;
                        match.PointsTeam2 = 0;
                    }
                    else
                    {
                        match.PointsTeam1 = 0;
                        match.PointsTeam2 = 3;
                    }
                }

                dbContext.Matches.Add(match);
                dbContext.SaveChanges();

                var matchDto = new MatchDto
                {
                    Team1 = team1.TeamName,
                    Team2 = team2.TeamName,
                    ScoreTeam1 = match.ScoreTeam1,
                    ScoreTeam2 = match.ScoreTeam2,
                    PointsTeam1 = match.PointsTeam1,
                    PointsTeam2 = match.PointsTeam2
                };

                return matchDto;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error in GenerateFinalMatch: {ex.InnerException?.Message ?? ex.Message}");
                throw; 
            }
        }

        public async Task<List<BestTeamsDto>> GetBestTeamsFromSemifinalsAsync()
        {
            try
            {
                var semifinalMatches = await dbContext.Matches
                    .OrderByDescending(m => m.PointsTeam1 + m.PointsTeam2)
                    .Take(2)
                    .ToListAsync();

                var bestTeams = new List<BestTeamsDto>();

                foreach (var match in semifinalMatches)
                {
                    var teamId = match.PointsTeam1 > match.PointsTeam2 ? match.Team1Id : match.Team2Id;
                    var team = await dbContext.Teams.FindAsync(teamId);

                    if (team != null)
                    {
                        var bestTeamDto = new BestTeamsDto
                        {
                            TeamId = team.TeamId,
                            TeamName = team.TeamName,
                            Points = match.PointsTeam1 > match.PointsTeam2 ? match.PointsTeam1 : match.PointsTeam2,
                            GroupId = match.GroupId
                        };

                        bestTeams.Add(bestTeamDto);
                    }
                }

                return bestTeams;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBestTeamsFromSemifinalsAsync: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }
        }

        public async Task<MatchDto> GenerateFinalAsync()
        {
            try
            {
                var latestMatches = await dbContext.Matches
                    .OrderByDescending(m => m.MatchId)
                    .Take(2)
                    .ToListAsync();

                if (latestMatches.Count < 2)
                {
                    throw new Exception("Cannot generate final match: Unable to find the latest two matches.");
                }


                var existingMatchesCount = await dbContext.Matches.CountAsync();

                if (existingMatchesCount != 26)
                {
                    throw new Exception("Cannot generate final game!");
                }

                var team1 = await dbContext.Teams.FindAsync(latestMatches[0].PointsTeam1 > latestMatches[0].PointsTeam2
                    ? latestMatches[0].Team1Id
                    : latestMatches[0].Team2Id);

                var team2 = await dbContext.Teams.FindAsync(latestMatches[1].PointsTeam1 > latestMatches[1].PointsTeam2
                    ? latestMatches[1].Team1Id
                    : latestMatches[1].Team2Id);


                var random = new Random();
                var scoreTeam1 = random.Next(0, 5);
                var scoreTeam2 = random.Next(0, 5);

                var finalMatchEntity = new Match
                {
                    Team1Id = team1.TeamId,
                    Team2Id = team2.TeamId,
                    ScoreTeam1 = scoreTeam1,
                    ScoreTeam2 = scoreTeam2,
                    PointsTeam1 = 0,
                    PointsTeam2 = 0,
                    GroupId = await dbContext.Groups   
                    .OrderBy(g => g.GroupId)  
                    .Select(g => g.GroupId)   
                    .FirstOrDefaultAsync()
            };

               

                if (scoreTeam1 > scoreTeam2)
                {
                    finalMatchEntity.PointsTeam1 = 3;
                }
                else if (scoreTeam1 < scoreTeam2)
                {
                    finalMatchEntity.PointsTeam2 = 3;
                }
                else
                {
                    // Scores are equal; randomly assign 3 points to one of the teams
                    var randomWinner = random.Next(0, 2); 

                    if (randomWinner == 0)
                    {
                        finalMatchEntity.PointsTeam1 = 3;
                    }
                    else
                    {
                        finalMatchEntity.PointsTeam2 = 3;
                    }
                }

                dbContext.Matches.Add(finalMatchEntity);
                await dbContext.SaveChangesAsync();

                var finalMatchDto = new MatchDto
                {
                    Team1 = team1.TeamName,
                    Team2 = team2.TeamName,
                    ScoreTeam1 = scoreTeam1,
                    ScoreTeam2 = scoreTeam2,
                    PointsTeam1 = finalMatchEntity.PointsTeam1,
                    PointsTeam2 = finalMatchEntity.PointsTeam2
                };

                return finalMatchDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateFinalMatchAsync: {ex}");
                throw;
            }
        }

        public async Task<string> GetWinnerAsync()
        {
            var finalMatch = await dbContext.Matches
                .OrderByDescending(m => m.MatchId)
                .FirstOrDefaultAsync();

            if (finalMatch == null)
                throw new Exception("Cannot determine winner: No final match found.");

            var winnerId = finalMatch.PointsTeam1 > finalMatch.PointsTeam2
                ? finalMatch.Team1Id
                : finalMatch.Team2Id;

            var winner = await dbContext.Teams.FindAsync(winnerId);

            if (winner == null)
                throw new Exception("Cannot determine winner: Unable to find the winning team.");

            return $"The winner is \"{winner.TeamName}\"!";
        }

    }
}

