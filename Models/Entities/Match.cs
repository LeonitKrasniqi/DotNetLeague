using System.ComponentModel.DataAnnotations;

namespace DotNetLeague.API.Models.Entities
{
    public class Match
    {
        [Key]
        public Guid MatchId { get; set; }

        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }

        public Guid Team1Id { get; set; }
        public virtual Team Team1 { get; set; }

        public Guid Team2Id { get; set; }
        public virtual Team Team2 { get; set; }

        public int ScoreTeam1 { get; set; }
        public int ScoreTeam2 { get; set; }

        public int PointsTeam1 { get; set; }
        public int PointsTeam2 { get; set; }
    }
}
