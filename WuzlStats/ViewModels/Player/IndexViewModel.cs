using System;
using System.Collections.Generic;
using System.Linq;
using WuzlStats.Models;

namespace WuzlStats.ViewModels.Player
{
    public class IndexViewModel
    {
        public IndexViewModel Build(Db db, string playerName)
        {
            Name = playerName;

            var scores =
                db.Scores.Where(x => x.TeamBlueDefensePlayer == playerName
                    || x.TeamBlueOffensePlayer == playerName
                    || x.TeamRedDefensePlayer == playerName
                    || x.TeamRedOffensePlayer == playerName)
                    .ToList();
            scores = scores.Where(x => x.TeamBlueDefensePlayer != x.TeamBlueOffensePlayer && x.TeamRedDefensePlayer != x.TeamRedOffensePlayer).ToList();


            if (scores.Count <= 0)
                throw new ArgumentOutOfRangeException("playerName", playerName, "The provided player seems to have no scores saved at all.");

            LastPlayedDate = scores.Max(x => x.Date).ToLocalTime();
            AllTimeStats = Calculate(scores);
            CurrentStats = Calculate(scores.Where(x => x.Date >= DateTime.UtcNow.AddDays(-30)).ToList());

            return this;
        }

        private PlayerStats Calculate(IList<Score> scores)
        {
            var result = new PlayerStats();

            var blueWins = scores.Where(x => x.TeamBlueScore > x.TeamRedScore).ToList();
            var redWins = scores.Where(x => x.TeamBlueScore < x.TeamRedScore).ToList();

            result.WinsCount = blueWins.Count(x => x.TeamBlueOffensePlayer == Name || x.TeamBlueDefensePlayer == Name)
                               + redWins.Count(x => x.TeamRedOffensePlayer == Name || x.TeamRedDefensePlayer == Name);
            result.LossesCount = blueWins.Count(x => x.TeamRedOffensePlayer == Name || x.TeamRedDefensePlayer == Name)
                                 + redWins.Count(x => x.TeamBlueOffensePlayer == Name || x.TeamBlueDefensePlayer == Name);


            var otherPlayers = (scores.Select(x => x.TeamBlueOffensePlayer)
                .Concat(scores.Select(x => x.TeamBlueDefensePlayer))
                .Concat(scores.Select(x => x.TeamRedOffensePlayer))
                .Concat(scores.Select(x => x.TeamRedDefensePlayer))).Where(x => x != Name);
            var otherPlayersGrouped = from x in otherPlayers
                                      group x by x
                                          into g
                                          orderby g.Count() descending
                                          select new
                                          {
                                              Name = g.Key,
                                              Count = g.Count()
                                          };
            result.FavoritePartner = otherPlayersGrouped.First().Name;


            var blueCount = scores.Count(x => x.TeamBlueDefensePlayer == Name || x.TeamBlueOffensePlayer == Name);
            var redCount = scores.Count() - blueCount;
            if (redCount == blueCount)
                result.FavoriteTeam = "No favorite team";
            else if (redCount < blueCount)
                result.FavoriteTeam = "Blue";
            else
                result.FavoriteTeam = "Red";

            var offenseCount = scores.Count(x => x.TeamRedOffensePlayer == Name || x.TeamBlueOffensePlayer == Name);
            var defenseCount = scores.Count() - offenseCount;
            if (defenseCount == offenseCount)
                result.FavoritePosition = "No favorite position";
            else if (defenseCount < offenseCount)
                result.FavoritePosition = "Offense";
            else
                result.FavoritePosition = "Defense";

            return result;
        }


        public string Name { get; set; }
        public DateTime LastPlayedDate { get; set; }
        public PlayerStats AllTimeStats { get; set; }
        public PlayerStats CurrentStats { get; set; }


        public class PlayerStats
        {
            public int WinsCount { get; set; }
            public int LossesCount { get; set; }
            public string FavoriteTeam { get; set; }
            public string FavoritePartner { get; set; }
            public string FavoritePosition { get; set; }
        }
    }
}