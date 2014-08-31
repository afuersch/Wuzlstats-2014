using System;
using System.Collections.Generic;

namespace WuzlStats.ViewModels
{
    public class ScoreViewModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<string> TeamBluePlayers { get; set; }
        public IEnumerable<string> TeamRedPlayers { get; set; }
        public string TeamBlue { get; set; }
        public string TeamRed { get; set; }
        public int BlueScore { get; set; }
        public int RedScore { get; set; }
    }
}