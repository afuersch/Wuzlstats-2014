using System.Web.Mvc;
using WuzlStats.Models;
using WuzlStats.ViewModels.Players;

namespace WuzlStats.Controllers
{
    public class PlayersController : Controller
    {
        private readonly Db _db = new Db();

        public ActionResult Index()
        {
            var viewModel = new IndexViewModel().Build(_db);
            return View(viewModel);
        }
	}
}