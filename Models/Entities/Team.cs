using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetLeague.API.Models.Entities
{
    public class Team
    {
        [Key]
        public Guid TeamId { get; set; }    
        public string TeamName { get; set; }
        public int NumberOfPlayers {  get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<League> League{ get; set; } = new List<League>();

    }
}
