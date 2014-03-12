using System;
using System.Collections.Generic;
using System.Linq;
using WuzlStats.Models;

namespace WuzlStats.ViewModels.Players
{
    public class IndexViewModel
    {
        public IndexViewModel Build(Db db)
        {
            Players = (from player in db.Players
                       let blueGames = player.Games.Where(x => x.Game.BlueScore > x.Game.RedScore)
                       let redGames = player.Games.Where(x => x.Game.BlueScore < x.Game.RedScore)
                       select new Player
                       {
                           Name = player.Name,
                           LastPlayedDate = player.Games.Max(x => x.Game.DateTime),
                           TeamGameWins = blueGames.Where(x => x.Team == Team.Blue).Count(x => x.Position == Position.Defense || x.Position == Position.Offense)
                                          + redGames.Where(x => x.Team == Team.Red).Count(x => x.Position == Position.Defense || x.Position == Position.Offense),
                           TeamGameLosses = blueGames.Where(x => x.Team == Team.Red).Count(x => x.Position == Position.Defense || x.Position == Position.Offense)
                                          + redGames.Where(x => x.Team == Team.Blue).Count(x => x.Position == Position.Defense || x.Position == Position.Offense),
                           SingleGameWins = blueGames.Where(x => x.Team == Team.Blue).Count(x => x.Position == Position.Both)
                                          + redGames.Where(x => x.Team == Team.Red).Count(x => x.Position == Position.Both),
                           SingleGameLosses = blueGames.Where(x => x.Team == Team.Red).Count(x => x.Position == Position.Both)
                                            + redGames.Where(x => x.Team == Team.Blue).Count(x => x.Position == Position.Both),
                       }).OrderByDescending(x => x.LastPlayedDate).ToList();

            foreach (var p in Players)
                p.LastPlayedDate = p.LastPlayedDate.ToLocalTime();

            return this;
        }

        public IEnumerable<Player> Players { get; set; }

        public class Player
        {
            public string Name { get; set; }
            public DateTime LastPlayedDate { get; set; }
            public int TeamGameWins { get; set; }
            public int TeamGameLosses { get; set; }
            public int SingleGameWins { get; set; }
            public int SingleGameLosses { get; set; }
        }
    }
}