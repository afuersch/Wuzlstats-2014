using System;
using System.Collections.Generic;
using System.Linq;
using WuzlStats.Models;

namespace WuzlStats.ViewModels.Home
{
    public class TeamResults
    {
        public TeamResults(Db db)
        {
            var minDate = DateTime.UtcNow.AddDays(-30);
            var games = from x in db.PlayerPositions
                        where x.Game.DateTime >= minDate && (x.Position == Position.Defense || x.Position == Position.Offense)
                        select x;

            CalculatePlayerResults(games);
            CalculateTeamResults(games);
        }

        private void CalculatePlayerResults(IQueryable<PlayerPosition> games)
        {
            var groupedGames = (from x in games
                                group x by x.Player.Name
                                    into g
                                    select new
                                    {
                                        PlayerName = g.Key,
                                        WinsAsDefense =
                                            g.Count(x => x.Position == Position.Defense && ((x.Team == Team.Blue && x.Game.BlueScore > x.Game.RedScore) || (x.Team == Team.Red && x.Game.BlueScore < x.Game.RedScore))),
                                        WinsAsOffense =
                                            g.Count(x => x.Position == Position.Offense && ((x.Team == Team.Blue && x.Game.BlueScore > x.Game.RedScore) || (x.Team == Team.Red && x.Game.BlueScore < x.Game.RedScore))),
                                        LossesAsDefense =
                                            g.Count(x => x.Position == Position.Defense && ((x.Team == Team.Blue && x.Game.BlueScore < x.Game.RedScore) || (x.Team == Team.Red && x.Game.BlueScore > x.Game.RedScore))),
                                        LossesAsOffense =
                                            g.Count(x => x.Position == Position.Offense && ((x.Team == Team.Blue && x.Game.BlueScore < x.Game.RedScore) || (x.Team == Team.Red && x.Game.BlueScore > x.Game.RedScore)))
                                    }).ToList();

            BestPlayers = groupedGames.Select(x => new PlayerResult
            {
                PlayerName = x.PlayerName,
                Wins = x.WinsAsDefense + x.WinsAsOffense,
                Losses = x.LossesAsDefense + x.LossesAsOffense
            }).OrderByDescending(x => x.Order);
            WorstPlayers = BestPlayers.OrderBy(x => x.Order);
            BestOffensePlayers = groupedGames.Select(x => new PlayerResult
            {
                PlayerName = x.PlayerName,
                Wins = x.WinsAsOffense,
                Losses = x.LossesAsOffense
            }).OrderByDescending(x => x.Order);
            BestDefensePlayers = groupedGames.Select(x => new PlayerResult
            {
                PlayerName = x.PlayerName,
                Wins = x.WinsAsDefense,
                Losses = x.LossesAsDefense
            }).OrderByDescending(x => x.Order);
            MostActivePlayers = groupedGames.Select(x => new PlayerResult
            {
                PlayerName = x.PlayerName,
                Wins = x.WinsAsDefense + x.WinsAsOffense,
                Losses = x.LossesAsDefense + x.LossesAsOffense
            }).OrderByDescending(x => x.Wins + x.Losses);
        }

        private void CalculateTeamResults(IQueryable<PlayerPosition> games)
        {
            var teams = from game in games.Select(x => x.Game).Distinct()
                        select new
                        {
                            BlueTeam = game.Players.Where(x => x.Team == Team.Blue).OrderBy(x => x.Position).Select(x => x.Player.Name),
                            RedTeam = game.Players.Where(x => x.Team == Team.Red).OrderBy(x => x.Position).Select(x => x.Player.Name),
                            BlueWins = game.BlueScore > game.RedScore
                        };


            var teamsResult = new List<TeamResult>();
            foreach (var team in teams)
            {
                var teamBlueResult = teamsResult.FirstOrDefault(x => x.DefensePlayerName == team.BlueTeam.ElementAt(1) && x.OffensePlayerName == team.BlueTeam.ElementAt(0));
                if (teamBlueResult == null)
                {
                    teamBlueResult = new TeamResult
                    {
                        DefensePlayerName = team.BlueTeam.ElementAt(1),
                        OffensePlayerName = team.BlueTeam.ElementAt(0)
                    };
                    teamsResult.Add(teamBlueResult);
                }
                var teamRedResult = teamsResult.FirstOrDefault(x => x.DefensePlayerName == team.RedTeam.ElementAt(1) && x.OffensePlayerName == team.RedTeam.ElementAt(0));
                if (teamRedResult == null)
                {
                    teamRedResult = new TeamResult
                    {
                        DefensePlayerName = team.RedTeam.ElementAt(1),
                        OffensePlayerName = team.RedTeam.ElementAt(0)
                    };
                    teamsResult.Add(teamRedResult);
                }

                if (team.BlueWins)
                {
                    teamBlueResult.Wins++;
                    teamRedResult.Losses++;
                }
                else
                {
                    teamBlueResult.Losses++;
                    teamRedResult.Wins++;
                }
            }

            BestTeams = teamsResult.OrderByDescending(x => x.Order);
        }

        public IEnumerable<PlayerResult> BestPlayers { get; set; }
        public IEnumerable<PlayerResult> WorstPlayers { get; set; }
        public IEnumerable<PlayerResult> BestOffensePlayers { get; set; }
        public IEnumerable<PlayerResult> BestDefensePlayers { get; set; }
        public IEnumerable<PlayerResult> MostActivePlayers { get; set; }

        public IEnumerable<TeamResult> BestTeams { get; set; }
    }
}