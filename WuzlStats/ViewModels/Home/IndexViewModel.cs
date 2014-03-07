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
            AllPlayers = (db.Scores.Select(x => x.TeamBlueOffensePlayer)
                .Concat(db.Scores.Select(x => x.TeamBlueDefensePlayer))
                .Concat(db.Scores.Select(x => x.TeamRedOffensePlayer))
                .Concat(db.Scores.Select(x => x.TeamRedDefensePlayer))).Distinct().OrderBy(x => x).ToList();

            var minDate = DateTime.UtcNow.AddDays(-30);
            var scores = db.Scores.Where(x => x.Date >= minDate && x.TeamBlueDefensePlayer != x.TeamBlueOffensePlayer && x.TeamRedDefensePlayer != x.TeamRedOffensePlayer).ToList();

            CalculatePlayerScores(scores);
            CalculateTeamScores(scores);

            return this;
        }


        private void CalculatePlayerScores(IList<Score> scores)
        {
            var blueWins = scores.Where(x => x.TeamBlueScore > x.TeamRedScore).ToList();
            var redWins = scores.Where(x => x.TeamBlueScore < x.TeamRedScore).ToList();

            var playersAndScores = AllPlayers.Select(player => new PlayerAndScore
            {
                Name = player,
                WinsOffense = blueWins.Where(x => x.TeamBlueOffensePlayer == player).Concat(redWins.Where(x => x.TeamRedOffensePlayer == player)).Count(),
                WinsDefense = blueWins.Where(x => x.TeamBlueDefensePlayer == player).Concat(redWins.Where(x => x.TeamRedDefensePlayer == player)).Count(),
                LossesOffense = redWins.Where(x => x.TeamBlueOffensePlayer == player).Concat(blueWins.Where(x => x.TeamRedOffensePlayer == player)).Count(),
                LossesDefense = redWins.Where(x => x.TeamBlueDefensePlayer == player).Concat(blueWins.Where(x => x.TeamRedDefensePlayer == player)).Count()
            }).ToList();

            BestPlayers = playersAndScores.OrderByDescending(x => x.Ratio);
            WorstPlayers = playersAndScores.OrderBy(x => x.Ratio);

            BestOffensePlayers = playersAndScores.OrderByDescending(x => x.OffenseRatio);
            BestDefensePlayers = playersAndScores.OrderByDescending(x => x.DefenseRatio);

            MostActivePlayers = playersAndScores.OrderByDescending(x => x.WinsDefense + x.WinsOffense + x.LossesDefense + x.LossesOffense);
        }

        private void CalculateTeamScores(IEnumerable<Score> scores)
        {
            var teams = new List<TeamAndScore>();
            foreach (var score in scores)
            {
                var blueTeam = teams.FirstOrDefault(x => x.OffensePlayerName == score.TeamBlueOffensePlayer && x.DefensePlayerName == score.TeamBlueDefensePlayer);
                var redTeam = teams.FirstOrDefault(x => x.OffensePlayerName == score.TeamRedOffensePlayer && x.DefensePlayerName == score.TeamRedDefensePlayer);
                if (blueTeam == null)
                {
                    blueTeam = new TeamAndScore
                    {
                        OffensePlayerName = score.TeamBlueOffensePlayer,
                        DefensePlayerName = score.TeamBlueDefensePlayer
                    };
                    teams.Add(blueTeam);
                }
                if (redTeam == null)
                {
                    redTeam = new TeamAndScore
                    {
                        OffensePlayerName = score.TeamRedOffensePlayer,
                        DefensePlayerName = score.TeamRedDefensePlayer
                    };
                    teams.Add(redTeam);
                }

                if (score.TeamBlueScore > score.TeamRedScore)
                {
                    blueTeam.Wins++;
                    redTeam.Losses++;
                }
                else if (score.TeamBlueScore < score.TeamRedScore)
                {
                    blueTeam.Losses++;
                    redTeam.Wins++;
                }
            }

            BestTeams = teams.OrderByDescending(x => x.Ratio);
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

        [Required]
        public string TeamBlueOffensePlayer { get; set; }

        [Required]
        public string TeamBlueDefensePlayer { get; set; }

        [Required]
        public int TeamBlueScore { get; set; }

        [Required]
        public string TeamRedOffensePlayer { get; set; }

        [Required]
        public string TeamRedDefensePlayer { get; set; }

        [Required]
        public int TeamRedScore { get; set; }

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