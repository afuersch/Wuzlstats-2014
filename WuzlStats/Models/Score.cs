using System;

namespace WuzlStats.Models
{
    public class Score
    {
        public int Id { get; set; }

        public string TeamBlueOffensePlayer { get; set; }
        public string TeamBlueDefensePlayer { get; set; }
        public int TeamBlueScore { get; set; }

        public string TeamRedOffensePlayer { get; set; }
        public string TeamRedDefensePlayer { get; set; }
        public int TeamRedScore { get; set; }

        public DateTime Date { get; set; }

    }
}