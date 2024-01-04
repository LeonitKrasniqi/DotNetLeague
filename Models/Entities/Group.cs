using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetLeague.API.Models.Entities
{
    public class Group
    {
        [Key]
        public Guid GroupId { get; set; }
        public char GroupName { get; set; }

        [JsonIgnore]
        public virtual ICollection<League> League { get; set; } = new List<League>();
        public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
        public virtual ICollection<Match> Matches { get; set; } = null!;

    }
}
