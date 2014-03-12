using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WuzlStats.Models
{
    public class PlayerPosition
    {
        [Key, Column(Order = 1)]
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        [Key, Column(Order = 0)]
        public int GameId { get; set; }

        public Game Game { get; set; }

        public Position Position { get; set; }
        public Team Team { get; set; }
    }

    public enum Position
    {
        Offense,
        Defense,
        Both
    }

    public enum Team
    {
        Blue,
        Red
    }
}