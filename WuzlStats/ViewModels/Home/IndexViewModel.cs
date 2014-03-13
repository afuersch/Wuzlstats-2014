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

            TeamResults = new TeamResults(db);
            SingleResults = new SingleResults(db);

            return this;
        }


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

        public IEnumerable<string> AllPlayers { get; set; }
        public TeamResults TeamResults { get; set; }
        public SingleResults SingleResults { get; set; }
    }
}