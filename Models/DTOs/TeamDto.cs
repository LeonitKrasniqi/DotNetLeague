namespace DotNetLeague.API.Models.DTOs
{
    public class TeamDto
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public int NumberOfPlayers { get; set; }
        public string Description { get; set; }
    }
}
