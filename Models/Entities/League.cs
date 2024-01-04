using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetLeague.API.Models.Entities
{
    public class League
    {
        public Guid TeamId {  get; set; }   
        public virtual Team Team { get; set; }  

        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }
    }
}
