using System.Collections.Generic;

namespace WuzlStats.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<PlayerPosition> Games { get; set; } 
    }
}