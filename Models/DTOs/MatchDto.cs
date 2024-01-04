namespace DotNetLeague.API.Models.DTOs
{
    public class MatchDto
    {
        public char GroupName { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }

        public int ScoreTeam1 { get; set; }
        public int ScoreTeam2 { get; set; }
        public int PointsTeam1 { get; set; }
        public int PointsTeam2 { get; set; }

    }
}
