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
                       orderby player.Name
                       select new Player
                       {
                           Name = player.Name,
                           NumberOfGames = player.Games.Count(),
                           LastPlayedDate = player.Games.Max(x => x.Game.DateTime)
                       }).ToList();

            foreach (var p in Players)
                p.LastPlayedDate = p.LastPlayedDate.ToLocalTime();

            return this;
        }

        public IEnumerable<Player> Players { get; set; }

        public class Player
        {
            public string Name { get; set; }
            public DateTime LastPlayedDate { get; set; }
            public int NumberOfGames { get; set; }
        }
    }
}