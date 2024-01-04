using System.ComponentModel.DataAnnotations;

namespace DotNetLeague.API.Models.DTOs
{
    public class AddTeamDto
    {
        [Required]
        public string TeamName { get; set; }
        [Required]
        [Range(23, 34, ErrorMessage = "Number of players must be between 23 and 34.")]
        public int NumberOfPlayers { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
