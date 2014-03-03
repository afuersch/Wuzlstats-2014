using System;
using System.Linq;
using System.Web.Mvc;
using WuzlStats.Models;
using WuzlStats.ViewModels.Home;

namespace WuzlStats.Controllers
{
    public class HomeController : Controller
    {
        readonly Db _db = new Db();

        public ActionResult Index()
        {
            var model = new IndexViewModel().Build(_db.Scores.OrderByDescending(x => x.Date).Take(20));


            var players = _db.Scores.Select(x => x.TeamBlueDefensePlayer)
                .Concat(_db.Scores.Select(x => x.TeamBlueOffensePlayer))
                .Concat(_db.Scores.Select(x => x.TeamRedDefensePlayer))
                .Concat(_db.Scores.Select(x => x.TeamRedOffensePlayer))
                .Distinct().OrderBy(x => x).ToList();
            model.Players = players;

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                var score = new Score
                {
                    TeamBlueDefensePlayer = model.TeamBlueDefensePlayer,
                    TeamBlueOffensePlayer = model.TeamBlueOffensePlayer,
                    TeamBlueScore = model.TeamBlueScore,
                    TeamRedDefensePlayer = model.TeamRedDefensePlayer,
                    TeamRedOffensePlayer = model.TeamRedOffensePlayer,
                    TeamRedScore = model.TeamRedScore,
                    Date = DateTime.Now
                };
                _db.Scores.Add(score);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
