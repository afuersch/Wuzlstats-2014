using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WuzlStats.ViewModels.Home
{
    public class IndexViewModel
    {
        public IndexViewModel Build(IEnumerable<Models.Score> scores)
        {
            var scoresList = scores.Select(s => new Score
            {
                TeamBlue = s.TeamBlueOffensePlayer + " / " + s.TeamBlueDefensePlayer,
                TeamRed = s.TeamRedOffensePlayer + " / " + s.TeamRedDefensePlayer,
                Outcome = s.TeamBlueScore + ":" + s.TeamRedScore
            }).ToList();

            Scores = scoresList;

            return this;
        }

        public IEnumerable<Score> Scores { get; set; }
        public IEnumerable<string> Players { get; set; }

        [Required]
        public string TeamBlueOffensePlayer { get; set; }
        [Required]
        public string TeamBlueDefensePlayer { get; set; }
        [Required]
        public int TeamBlueScore { get; set; }

        [Required]
        public string TeamRedOffensePlayer { get; set; }
        [Required]
        public string TeamRedDefensePlayer { get; set; }
        [Required]
        public int TeamRedScore { get; set; }

        public class Score
        {
            public string TeamBlue { get; set; }
            public string TeamRed { get; set; }
            public string Outcome { get; set; }
        }
    }
}