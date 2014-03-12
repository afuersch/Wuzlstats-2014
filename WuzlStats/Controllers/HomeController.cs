using System;
using System.Globalization;
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
            return View(new IndexViewModel().Build(_db));
        }

        [HttpPost]
        public ActionResult Teams(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                var game = new Game
                {
                    BlueScore = model.BlueScore,
                    RedScore = model.RedScore,
                    DateTime = DateTime.UtcNow
                };
                _db.Games.Add(game);

                _db.PlayerPositions.Add(new PlayerPosition
                {
                    Game = game,
                    Team = Team.Blue,
                    Position = Position.Defense,
                    PlayerId = GetOrCreatePlayer(model.BlueDefensePlayer).Id
                });
                _db.PlayerPositions.Add(new PlayerPosition
                {
                    Game = game,
                    Team = Team.Blue,
                    Position = Position.Offense,
                    PlayerId = GetOrCreatePlayer(model.BlueOffensePlayer).Id
                });
                _db.PlayerPositions.Add(new PlayerPosition
                {
                    Game = game,
                    Team = Team.Red,
                    Position = Position.Defense,
                    PlayerId = GetOrCreatePlayer(model.RedDefensePlayer).Id
                });
                _db.PlayerPositions.Add(new PlayerPosition
                {
                    Game = game,
                    Team = Team.Red,
                    Position = Position.Offense,
                    PlayerId = GetOrCreatePlayer(model.RedOffensePlayer).Id
                });

                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult Singles(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                var game = new Game
                {
                    BlueScore = model.BlueScore,
                    RedScore = model.RedScore,
                    DateTime = DateTime.UtcNow
                };
                _db.Games.Add(game);

                _db.PlayerPositions.Add(new PlayerPosition
                {
                    Game = game,
                    Team = Team.Blue,
                    Position = Position.Both,
                    PlayerId = GetOrCreatePlayer(model.BluePlayer).Id
                });
                _db.PlayerPositions.Add(new PlayerPosition
                {
                    Game = game,
                    Team = Team.Red,
                    Position = Position.Both,
                    PlayerId = GetOrCreatePlayer(model.RedPlayer).Id
                });
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View("Index");
        }

        private string NormalizeName(string name)
        {
            if (name == null || name.Length <= 1)
                return name ?? "";

            var result = name.Substring(1).ToLowerInvariant();
            result = name[0].ToString(CultureInfo.InvariantCulture).ToUpperInvariant() + result;
            return result;
        }

        private Player GetOrCreatePlayer(string name)
        {
            name = NormalizeName(name);
            var player = _db.Players.FirstOrDefault(x => x.Name == name);
            if (player == null)
            {
                player = new Player
                {
                    Name = name
                };
                _db.Players.Add(player);
                _db.SaveChanges();
            }
            return player;
        }
    }
}
