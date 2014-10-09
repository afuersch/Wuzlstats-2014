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

            var games = db.PlayerPositions.Where(x => x.Player.Name == playerName);
            var teamGames = games.Where(x => x.Position != Position.Both);
            var singleGames = games.Where(x => x.Position == Position.Both);

            LastPlayedDate = games.Max(x => x.Game.DateTime).ToLocalTime();
            AllTimeStats = CalculateTeamPlayerStats(teamGames);
            CurrentStats = CalculateTeamPlayerStats(teamGames.Where(x => x.Game.DateTime >= MinDate));

            AllTimeSingleStats = CalculateSinglePlayerStats(singleGames);
            CurrentSingleStats = CalculateSinglePlayerStats(singleGames.Where(x => x.Game.DateTime >= MinDate));

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
                s.TeamBlue = string.Join(" / ", s.TeamBluePlayers);
                s.TeamRed = string.Join(" / ", s.TeamRedPlayers);
            }

            return this;
        }

        private SinglePlayerStats CalculateSinglePlayerStats(IQueryable<PlayerPosition> games)
        {
            var result = new SinglePlayerStats();
            Calculate(games, result);

            var opponents = games.SelectMany(x => x.Game.Players.Where(y => y.Team != x.Team && y.Player != x.Player).Select(z => z.Player.Name)).ToList();

            result.FavoriteOpponent = (from x in opponents
                                       group x by x
                                           into g
                                           orderby g.Count() descending
                                           select g.Key)
                 .First();

            return result;
        }

        private TeamPlayerStats CalculateTeamPlayerStats(IQueryable<PlayerPosition> games)
        {
            var result = new TeamPlayerStats();
            Calculate(games, result);
            var otherPlayers = games.SelectMany(x => x.Game.Players.Where(y => y.Team == x.Team && y.Player != x.Player)).Select(x => x.Player.Name);
            result.FavoritePartner = (from x in otherPlayers
                                      group x by x
                                          into g
                                          orderby g.Count() descending
                                          select new
                                          {
                                              Name = g.Key
                                          }).First().Name;

            var offenseCount = games.Count(x => x.Position == Position.Offense);
            var defenseCount = games.Count(x => x.Position == Position.Defense);
            if (defenseCount == offenseCount)
            {
                result.FavoritePosition = "No favorite position";
            }
            else if (defenseCount < offenseCount)
            {
                result.FavoritePosition = "Offense";
            }
            else
            {
                result.FavoritePosition = "Defense";
            }

            var opponents = (from players in
                                 (from a in games.Select(x => x.Game.Players.Where(y => y.Team != x.Team && y.Player != x.Player))
                                  select new
                                  {
                                      OffensePlayer = a.FirstOrDefault(p => p.Position == Position.Offense).Player.Name,
                                      DefensePlayer = a.FirstOrDefault(p => p.Position == Position.Defense).Player.Name
                                  })
                             select players.OffensePlayer + " / " + players.DefensePlayer)
                 .ToList();

            result.FavoriteOpponent = (from x in opponents
                                       group x by x
                                           into g
                                           orderby g.Count() descending
                                           select g.Key)
                 .First();

            return result;
        }

        private void Calculate(IQueryable<PlayerPosition> games, IPlayerStats playerStats)
        {
            var blueWins = games.Where(x => x.Game.BlueScore > x.Game.RedScore).ToList();
            var redWins = games.Where(x => x.Game.BlueScore < x.Game.RedScore).ToList();

            playerStats.WinsCount = blueWins.Count(x => x.Team == Team.Blue) + redWins.Count(x => x.Team == Team.Red);
            playerStats.LossesCount = blueWins.Count(x => x.Team == Team.Red) + redWins.Count(x => x.Team == Team.Blue);

            var blueCount = games.Count(x => x.Team == Team.Blue);
            var redCount = games.Count(x => x.Team == Team.Red);
            if (redCount == blueCount)
            {
                playerStats.FavoriteTeam = "None";
            }
            else if (redCount < blueCount)
            {
                playerStats.FavoriteTeam = "Blue";
            }
            else
            {
                playerStats.FavoriteTeam = "Red";
            }
        }

        public string Name { get; set; }
        public DateTime LastPlayedDate { get; set; }
        public DateTime MinDate
        {
            get
            {
                return DateTime.UtcNow.AddDays(-30);
            }
        }
        public TeamPlayerStats AllTimeStats { get; set; }
        public TeamPlayerStats CurrentStats { get; set; }
        public SinglePlayerStats AllTimeSingleStats { get; set; }
        public SinglePlayerStats CurrentSingleStats { get; set; }
        public IEnumerable<ScoreViewModel> Scores { get; set; }

    }
}