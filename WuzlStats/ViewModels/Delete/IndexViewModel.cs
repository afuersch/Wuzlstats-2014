using System;
using System.Collections.Generic;
using System.Linq;
using WuzlStats.Models;

namespace WuzlStats.ViewModels.Delete
{
    public class IndexViewModel
    {
        public IndexViewModel Build(Db db)
        {
            var games = db.Games.OrderByDescending(x => x.DateTime).Take(50);

            Scores = (from game in games
                      select new ScoreViewModel
                      {
                          Id = game.Id,
                          DateTime = game.DateTime,
                          RedScore = game.RedScore,
                          BlueScore = game.BlueScore,
                          TeamBluePlayers = game.Players.Where(x => x.Team == Team.Blue).OrderBy(x => x.Position).Select(x => x.Player.Name).ToList(),
                          TeamRedPlayers = game.Players.Where(x => x.Team == Team.Red).OrderBy(x => x.Position).Select(x => x.Player.Name).ToList(),
                      }).ToList();

            foreach (var s in Scores)
            {
                s.DateTime = s.DateTime.ToLocalTime();
                s.TeamBlue = s.TeamBluePlayers.Aggregate("", (sum, current) => sum + " / " + current).Trim(' ', '/');
                s.TeamRed = s.TeamRedPlayers.Aggregate("", (sum, current) => sum + " / " + current).Trim(' ', '/');
            }

            return this;
        }

        public IEnumerable<ScoreViewModel> Scores { get; set; }

    }
}