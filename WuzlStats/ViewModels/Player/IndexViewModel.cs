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

            var games = db.PlayerPositions.Where(x => x.Player.Name == playerName && x.Position != Position.Both);

            var minDate = DateTime.UtcNow.AddDays(-30);
            LastPlayedDate = games.Max(x => x.Game.DateTime).ToLocalTime();
            AllTimeStats = Calculate(games);
            CurrentStats = Calculate(games.Where(x => x.Game.DateTime >= minDate));

            var player = db.Players.FirstOrDefault(p => p.Name == playerName);

            Scores = (from game in db.Games.Where(game => game.Players.FirstOrDefault(position => position.PlayerId == player.Id) != null).OrderByDescending(x => x.DateTime)
                      select new ScoreViewModel
                      {
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

        private TeamPlayerStats Calculate(IQueryable<PlayerPosition> games)
        {
            var result = new TeamPlayerStats();

            var blueWins = games.Where(x => x.Game.BlueScore > x.Game.RedScore).ToList();
            var redWins = games.Where(x => x.Game.BlueScore < x.Game.RedScore).ToList();

            result.WinsCount = blueWins.Count(x => x.Team == Team.Blue) + redWins.Count(x => x.Team == Team.Red);
            result.LossesCount = blueWins.Count(x => x.Team == Team.Red) + redWins.Count(x => x.Team == Team.Blue);

            var otherPlayers = games.SelectMany(x => x.Game.Players.Where(y => y.Team == x.Team && y.Player != x.Player)).Select(x => x.Player.Name);
            result.FavoritePartner = (from x in otherPlayers
                                      group x by x
                                          into g
                                          orderby g.Count() descending
                                          select new
                                          {
                                              Name = g.Key
                                          }).First().Name;


            var blueCount = games.Count(x => x.Team == Team.Blue);
            var redCount = games.Count(x => x.Team == Team.Red);
            if (redCount == blueCount)
                result.FavoriteTeam = "No favorite team";
            else if (redCount < blueCount)
                result.FavoriteTeam = "Blue";
            else
                result.FavoriteTeam = "Red";

            var offenseCount = games.Count(x => x.Position == Position.Offense);
            var defenseCount = games.Count(x => x.Position == Position.Defense);
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
        public TeamPlayerStats AllTimeStats { get; set; }
        public TeamPlayerStats CurrentStats { get; set; }
        public IEnumerable<ScoreViewModel> Scores { get; set; }

    }
}