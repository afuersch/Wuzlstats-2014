using System;
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
            return View(new IndexViewModel().Build(_db));
        }

        [HttpPost]
        public ActionResult Index(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                var score = new Score
                {
                    TeamBlueDefensePlayer = NormalizeName(model.TeamBlueDefensePlayer),
                    TeamBlueOffensePlayer = NormalizeName(model.TeamBlueOffensePlayer),
                    TeamBlueScore = model.TeamBlueScore,
                    TeamRedDefensePlayer = NormalizeName(model.TeamRedDefensePlayer),
                    TeamRedOffensePlayer = NormalizeName(model.TeamRedOffensePlayer),
                    TeamRedScore = model.TeamRedScore,
                    Date = DateTime.UtcNow
                };
                _db.Scores.Add(score);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View();
        }

        private string NormalizeName(string name)
        {
            if (name == null || name.Length <= 1)
                return name ?? "";

            var result = name.Substring(1).ToLowerInvariant();
            result = name[0].ToString().ToUpperInvariant() + result;
            return result;
        }
    }
}
