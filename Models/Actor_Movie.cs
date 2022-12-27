using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.Models
{
    public class Actor_Movie
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Movie")]
        public int MovieId { get; set; }
      
        public Movie? Movie { get; set; }
        [ForeignKey("Actor")]
        public int ActorId { get; set; }
       
        public Actor? Actor { get; set; }
    }
}
