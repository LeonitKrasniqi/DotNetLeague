namespace DotNetLeague.API.Models.DTOs
{
    public class BestTeamsDto
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public int Points { get; set; }
        public Guid GroupId { get; set; }
    }
}
