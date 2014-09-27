using System;
using System.Linq;
using System.Web.Mvc;
using WuzlStats.Models;
using WuzlStats.ViewModels.Statistics;

namespace WuzlStats.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly Db _db = new Db();

        public ActionResult Index()
        {
            //build model
            var players = from player in _db.Players
                          select new SelectListItem
                          {
                              Text = player.Name,
                              Value = player.Name
                          };

            IndexViewModel model = new IndexViewModel
            {
                End = DateTime.Today,
                Players = players
            };
            return View(model);
        }
    }
}