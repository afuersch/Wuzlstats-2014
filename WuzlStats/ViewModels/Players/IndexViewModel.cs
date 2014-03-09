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
            var playerNames = (db.Scores.Select(x => x.TeamBlueOffensePlayer)
                .Concat(db.Scores.Select(x => x.TeamBlueDefensePlayer))
                .Concat(db.Scores.Select(x => x.TeamRedOffensePlayer))
                .Concat(db.Scores.Select(x => x.TeamRedDefensePlayer))).Distinct().OrderBy(x => x).ToList();

            Players = (from playerName in playerNames
                       let playerScores =
                           db.Scores.Where(x => (x.TeamBlueDefensePlayer == playerName || x.TeamBlueOffensePlayer == playerName || x.TeamRedDefensePlayer == playerName || x.TeamRedOffensePlayer == playerName) && x.TeamBlueDefensePlayer != x.TeamBlueOffensePlayer && x.TeamRedDefensePlayer != x.TeamRedOffensePlayer)
                       select new Player
                       {
                           Name = playerName,
                           LastPlayedDate = playerScores.Max(x => x.Date),
                           NumberOfGames = playerScores.Count()
                       }).ToList();

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