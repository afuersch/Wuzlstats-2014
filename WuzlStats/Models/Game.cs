using System;
using System.Collections.Generic;

namespace WuzlStats.Models
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int BlueScore { get; set; }
        public int RedScore { get; set; }
        public virtual ICollection<PlayerPosition> Players { get; set; } 
    }
}