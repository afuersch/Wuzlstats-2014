using System;
using System.Collections.Generic;
using System.Linq;
using WuzlStats.Models;

namespace WuzlStats.ViewModels.Home
{
    public class SingleResults
    {
        public SingleResults(Db db)
        {
            var minDate = DateTime.UtcNow.AddDays(-30);

            var games = from x in db.PlayerPositions
                        where x.Position == Position.Both && x.Game.DateTime >= minDate
                        select x;


            var players = (from x in games
                           group x by x.Player.Name
                               into g
                               select new PlayerAndScore
                               {
                                   PlayerName = g.Key,
                                   Wins = g.Count(y => (y.Team == Team.Blue && y.Game.BlueScore > y.Game.RedScore) || (y.Team == Team.Red && y.Game.BlueScore < y.Game.RedScore)),
                                   Losses = g.Count(y => (y.Team == Team.Blue && y.Game.BlueScore < y.Game.RedScore) || (y.Team == Team.Red && y.Game.BlueScore > y.Game.RedScore))
                               }).ToList();

            BestPlayers = players.OrderByDescending(x => x.Order);
            WorstPlayers = players.OrderBy(x => x.Order);
            MostActivePlayers = players.OrderByDescending(x => x.Wins + x.Losses);
        }

        public IEnumerable<PlayerAndScore> BestPlayers { get; set; }
        public IEnumerable<PlayerAndScore> WorstPlayers { get; set; }
        public IEnumerable<PlayerAndScore> MostActivePlayers { get; set; }

        public class PlayerAndScore
        {
            public string PlayerName { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }

            public double Order
            {
                get
                {
                    var wins = (double)Wins;
                    var losses = (double)Losses;

                    if (losses <= 0)
                        return wins;
                    if (wins <= 0)
                        return 0.1 / losses;
                    return wins / losses;
                }
            }
        }
    }
}