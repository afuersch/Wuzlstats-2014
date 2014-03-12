using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WuzlStats.Models;

namespace WuzlStats.ViewModels.Home
{
    public class IndexViewModel
    {
        public IndexViewModel Build(Db db)
        {
            AllPlayers = db.Players.Select(x => x.Name).OrderBy(x => x);
            var minDate = DateTime.UtcNow.AddDays(-30);

            #region Player scores

            var playersAndScores = (from player in db.Players
                                    let games = player.Games.Where(x => x.Game.DateTime >= minDate)
                                    let wonGames = games.Where(x => (x.Game.BlueScore > x.Game.RedScore && x.Team == Team.Blue) || (x.Game.BlueScore < x.Game.RedScore && x.Team == Team.Red))
                                    let lostGames = games.Where(x => (x.Game.BlueScore < x.Game.RedScore && x.Team == Team.Blue) || (x.Game.BlueScore > x.Game.RedScore && x.Team == Team.Red))
                                    select new PlayerAndScore
                                    {
                                        Name = player.Name,
                                        LossesDefense = lostGames.Count(x => x.Position == Position.Defense),
                                        LossesOffense = lostGames.Count(x => x.Position == Position.Offense),
                                        WinsDefense = wonGames.Count(x => x.Position == Position.Defense),
                                        WinsOffense = wonGames.Count(x => x.Position == Position.Offense)
                                    }).ToList();

            BestPlayers = playersAndScores.OrderByDescending(x => x.Ratio);
            WorstPlayers = playersAndScores.OrderBy(x => x.Ratio);

            BestOffensePlayers = playersAndScores.OrderByDescending(x => x.OffenseRatio);
            BestDefensePlayers = playersAndScores.OrderByDescending(x => x.DefenseRatio);

            MostActivePlayers = playersAndScores.OrderByDescending(x => x.WinsDefense + x.WinsOffense + x.LossesDefense + x.LossesOffense);

            #endregion

            #region Team scores

            var teams = (from game in db.Games
                         where game.DateTime >= minDate
                         select new
                         {
                             BlueTeam = game.Players.Where(x => x.Team == Team.Blue).OrderBy(x => x.Position).Select(x => x.Player).ToList(),
                             RedTeam = game.Players.Where(x => x.Team == Team.Red).OrderBy(x => x.Position).Select(x => x.Player).ToList(),
                             BlueWins = game.BlueScore > game.RedScore
                         }).ToList();

            var teamsResult = new List<TeamAndScore>();
            foreach (var team in teams)
            {
                if (team.BlueTeam.Count <= 1 || team.RedTeam.Count <= 1)
                {
                    // do nothing here, not support for single-player yet
                }
                else
                {
                    var teamBlueResult = teamsResult.FirstOrDefault(x => x.DefensePlayerName == team.BlueTeam.ElementAt(1).Name && x.OffensePlayerName == team.BlueTeam.ElementAt(0).Name);
                    if (teamBlueResult == null)
                    {
                        teamBlueResult = new TeamAndScore
                        {
                            DefensePlayerName = team.BlueTeam.ElementAt(1).Name,
                            OffensePlayerName = team.BlueTeam.ElementAt(0).Name
                        };
                        teamsResult.Add(teamBlueResult);
                    }
                    var teamRedResult = teamsResult.FirstOrDefault(x => x.DefensePlayerName == team.RedTeam.ElementAt(1).Name && x.OffensePlayerName == team.RedTeam.ElementAt(0).Name);
                    if (teamRedResult == null)
                    {
                        teamRedResult = new TeamAndScore
                        {
                            DefensePlayerName = team.RedTeam.ElementAt(1).Name,
                            OffensePlayerName = team.RedTeam.ElementAt(0).Name
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
            }
            BestTeams = teamsResult.OrderByDescending(x=>x.Ratio);

            #endregion

            return this;
        }

        #region Output properties

        public IEnumerable<PlayerAndScore> BestPlayers { get; set; }
        public IEnumerable<PlayerAndScore> WorstPlayers { get; set; }
        public IEnumerable<PlayerAndScore> BestOffensePlayers { get; set; }
        public IEnumerable<PlayerAndScore> BestDefensePlayers { get; set; }

        public IEnumerable<PlayerAndScore> MostActivePlayers { get; set; }

        public IEnumerable<TeamAndScore> BestTeams { get; set; }

        public IEnumerable<string> AllPlayers { get; set; }

        #endregion

        #region Input properties

        public string BlueOffensePlayer { get; set; }
        public string BlueDefensePlayer { get; set; }
        public string BluePlayer { get; set; }

        public string RedOffensePlayer { get; set; }
        public string RedDefensePlayer { get; set; }
        public string RedPlayer { get; set; }

        [Required]
        public int BlueScore { get; set; }
        [Required]
        public int RedScore { get; set; }

        #endregion

        public class PlayerAndScore
        {
            public int WinsOffense { get; set; }
            public int WinsDefense { get; set; }
            public int LossesOffense { get; set; }
            public int LossesDefense { get; set; }
            public string Name { get; set; }

            public double Ratio
            {
                get
                {
                    var wins = (double)WinsOffense + WinsDefense;
                    var losses = (double)LossesOffense + LossesDefense;

                    if (losses > 0)
                        return wins / losses;
                    return wins;
                }
            }

            public double OffenseRatio
            {
                get
                {
                    var wins = (double)WinsOffense;
                    var losses = (double)LossesOffense;

                    if (losses > 0)
                        return wins / losses;
                    return wins;
                }
            }

            public double DefenseRatio
            {
                get
                {
                    var wins = (double)WinsDefense;
                    var losses = (double)LossesDefense;

                    if (losses > 0)
                        return wins / losses;
                    return wins;
                }
            }
        }

        public class TeamAndScore
        {
            public int Wins { get; set; }
            public int Losses { get; set; }
            public string OffensePlayerName { get; set; }
            public string DefensePlayerName { get; set; }

            public double Ratio
            {
                get
                {
                    var wins = (double)Wins;
                    var losses = (double)Losses;

                    if (losses > 0)
                        return wins / losses;
                    return wins;
                }
            }
        }
    }
}