using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WuzlStats.Models;

namespace WuzlStats.ViewModels.Statistics
{
    public class IndexViewModel
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public IEnumerable<SelectListItem> Players { get; set; }
        public Team? Team1Color { get; set; }
        public Team? Team2Color { get; set; }
        public string Team1Player1 { get; set; }
        public string Team1Player2 { get; set; }
        public string Team2Player1 { get; set; }
        public string Team2Player2 { get; set; }
    }
}