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
                               select new PlayerResult
                               {
                                   PlayerName = g.Key,
                                   Wins = g.Count(y => (y.Team == Team.Blue && y.Game.BlueScore > y.Game.RedScore) || (y.Team == Team.Red && y.Game.BlueScore < y.Game.RedScore)),
                                   Losses = g.Count(y => (y.Team == Team.Blue && y.Game.BlueScore < y.Game.RedScore) || (y.Team == Team.Red && y.Game.BlueScore > y.Game.RedScore))
                               }).ToList();

            BestPlayers = players.OrderByDescending(x => x.Order);
            WorstPlayers = players.OrderBy(x => x.Order);
            MostActivePlayers = players.OrderByDescending(x => x.Wins + x.Losses);

            // ReSharper disable ConstantNullCoalescingCondition
            var goals = (from x in games
                         group x by x.Player.Name
                             into g
                             select new PlayerResult
                             {
                                 PlayerName = g.Key,
                                 Wins = (g.Where(y => y.Team == Team.Blue).Sum(y => (int?)y.Game.BlueScore) ?? 0) + (g.Where(y => y.Team == Team.Red).Sum(y => (int?)y.Game.RedScore) ?? 0),
                                 Losses = (g.Where(y => y.Team == Team.Red).Sum(y => (int?)y.Game.BlueScore) ?? 0) + (g.Where(y => y.Team == Team.Blue).Sum(y => (int?)y.Game.RedScore) ?? 0),
                             }).ToList();
            // ReSharper restore ConstantNullCoalescingCondition
            MostGoals = goals.OrderByDescending(x => x.Order);
        }

        public IEnumerable<PlayerResult> BestPlayers { get; set; }
        public IEnumerable<PlayerResult> WorstPlayers { get; set; }
        public IEnumerable<PlayerResult> MostActivePlayers { get; set; }
        public IEnumerable<PlayerResult> MostGoals { get; set; }
    }
}