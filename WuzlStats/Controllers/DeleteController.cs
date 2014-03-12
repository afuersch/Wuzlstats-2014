using System.Linq;
using System.Web.Mvc;
using WuzlStats.Models;
using WuzlStats.ViewModels.Delete;

namespace WuzlStats.Controllers
{
    public class DeleteController : Controller
    {
        private readonly Db _db = new Db();

        [Route("Delete")]
        public ActionResult Index()
        {
            return View(new IndexViewModel().Build(_db));
        }

        [Route("Delete/{gameId}")]
        public ActionResult Index(int gameId)
        {
            var game = _db.Games.Find(gameId);
            if (game != null)
            {
                _db.Games.Remove(game);
                _db.SaveChanges();

                // remove all players that don't have any games left (to clean up the database if necessary)
                foreach (var p in _db.Players.Where(x => !x.Games.Any()))
                    _db.Players.Remove(p);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
