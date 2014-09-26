using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WuzlStats.ViewModels.Statistics
{
    public class IndexViewModel
    {
        public IEnumerable<string> Players { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}