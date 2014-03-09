using System;
using System.Collections.Generic;
using System.Linq;

namespace WuzlStats.ViewModels.Delete
{
    public class IndexViewModel
    {
        public IndexViewModel Build(IEnumerable<Models.Score> scores)
        {
            var scoresList = scores.Select(s => new Score
            {
                TeamBlue = s.TeamBlueOffensePlayer + " / " + s.TeamBlueDefensePlayer,
                TeamRed = s.TeamRedOffensePlayer + " / " + s.TeamRedDefensePlayer,
                BlueScore = s.TeamBlueScore,
                RedScore = s.TeamRedScore,
                Id = s.Id,
                DateTime = s.Date
            }).ToList();

            Scores = scoresList;

            return this;
        }

        public IEnumerable<Score> Scores { get; set; }

        public class Score
        {
            public int Id { get; set; }
            public DateTime DateTime { get; set; }
            public string TeamBlue { get; set; }
            public string TeamRed { get; set; }
            public int BlueScore { get; set; }
            public int RedScore { get; set; }
        }

    }
}